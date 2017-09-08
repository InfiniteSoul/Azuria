﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Azuria.Api.v1.Input.Converter;
using Azuria.Exceptions;
using Azuria.Helpers.Attributes;
using Azuria.Helpers.Extensions;

namespace Azuria.Api.v1.Input
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public abstract class InputDataModel : IInputDataModel
    {
        /// <inheritdoc />
        public virtual Dictionary<string, string> Build()
        {
            IEnumerable<Tuple<PropertyInfo, InputDataAttribute>> lInputDataProperties = this.GetType()
                .GetRuntimeProperties()
                .Select(
                    info => new Tuple<PropertyInfo, InputDataAttribute>(
                        info, info.GetCustomAttribute<InputDataAttribute>()
                    )
                ).Where(tuple => tuple.Item2 != null);

            Dictionary<string, string> lReturn = new Dictionary<string, string>();
            lReturn.AddOrUpdateRange(
                lInputDataProperties.Select(
                    tuple => new KeyValuePair<string, string>(
                        tuple.Item2.Key, this.GetPropertyValue(tuple.Item1, tuple.Item2)
                    )
                ).Where(pair => pair.Value != null)
            );
            return lReturn;
        }

        private string GetPropertyValue(PropertyInfo propertyInfo, InputDataAttribute attribute)
        {
            if (this.ContainsForbiddenValue(propertyInfo, attribute))
                return attribute.Optional
                           ? (string) null
                           : throw new InvalidOperationException(
                                 $"This value is not supported for {propertyInfo.Name}"
                             );
            if (string.IsNullOrWhiteSpace(attribute.ConverterMethodName) && attribute.Converter == null)
                return propertyInfo.GetValue(this)?.ToString() ?? (attribute.Optional ? null : string.Empty);

            Func<object, string> lConversionMethod = this.FindConversionMethod(propertyInfo, attribute);

            if (lConversionMethod != null)
            {
                string lConvertedValue = lConversionMethod.Invoke(propertyInfo.GetValue(this));
                if (lConvertedValue != null && (attribute.ForbiddenValues == null ||
                                                !attribute.ForbiddenValues.Contains(lConvertedValue)))
                    return lConvertedValue;
                if (attribute.Optional) return null;
                throw new InvalidOperationException($"This value is not supported for {propertyInfo.Name}");
            }
            if (attribute.Optional) return null;

            throw new MethodNotFoundException(attribute.ConverterMethodName);
        }

        private Func<object, string> FindConversionMethod(PropertyInfo propertyInfo, InputDataAttribute attribute)
        {
            MethodInfo FindFromName(Type type, string methodName)
            {
                if (string.IsNullOrWhiteSpace(methodName)) return null;
                MethodInfo[] lMethods = type.GetRuntimeMethods()
                    .Where(info => info.Name.Equals(methodName))
                    .Where(info => info.ReturnType == typeof(string))
                    .Where(info => info.GetParameters().Length == 1).ToArray();
                return lMethods.FirstOrDefault(
                    info => info.GetParameters().First().ParameterType.GetTypeInfo().IsAssignableFrom(
                        GetNullableType(propertyInfo.PropertyType).GetTypeInfo()
                    )
                );
            }

            MethodInfo lConverterMethod = FindFromName(this.GetType(), attribute.ConverterMethodName);
            if (lConverterMethod != null)
                return o => lConverterMethod.Invoke(this, new[] {o})?.ToString();
            if (attribute.Converter == null ||
                !ImplementsInputDataConverter(attribute.Converter.GetTypeInfo(), propertyInfo.PropertyType))
                return null;

            lConverterMethod = FindFromName(attribute.Converter, "Convert");
            object lConverterInstance = Activator.CreateInstance(attribute.Converter);
            if (lConverterMethod != null)
                return o => lConverterMethod.Invoke(lConverterInstance, new[] {o}).ToString();

            return null;
        }

        private bool ContainsForbiddenValue(PropertyInfo propertyInfo, InputDataAttribute attribute)
        {
            if (attribute.ForbiddenValues == null || attribute.ForbiddenValues.Length == 0) return false;
            object lPropertyValue = propertyInfo.GetValue(this);
            return attribute.ForbiddenValues.Any(forbiddenValue => forbiddenValue.Equals(lPropertyValue));
        }

        private static bool ImplementsInputDataConverter(TypeInfo type, Type dataType)
        {
            return type.ImplementedInterfaces.Select(type1 => type1.GetTypeInfo())
                .Where(info => info.IsGenericType && info.GetGenericTypeDefinition() == typeof(IInputDataConverter<>))
                .SelectMany(info => info.GenericTypeArguments)
                .Any(type1 => type1.GetTypeInfo().IsAssignableFrom(GetNullableType(dataType).GetTypeInfo()));
        }

        private static Type GetNullableType(Type nullable)
        {
            return Nullable.GetUnderlyingType(nullable) ?? nullable;
        }
    }
}