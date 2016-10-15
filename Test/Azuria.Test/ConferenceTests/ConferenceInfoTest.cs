﻿using System.Linq;
using System.Threading.Tasks;
using Azuria.Community;
using Azuria.Utilities.Extensions;
using NUnit.Framework;

namespace Azuria.Test.ConferenceTests
{
    public class ConferenceInfoTest
    {
        private ConferenceInfo _conferenceInfo;

        #region Methods

        [Test]
        public void ConferenceTest()
        {
            Assert.IsNotNull(this._conferenceInfo.Conference);
            Assert.AreEqual(124536, this._conferenceInfo.Conference.Id);
        }

        [Test]
        public void GetUnreadMessages()
        {
            Message[] lUnreadMessages = this._conferenceInfo.UnreadMessages.ToArray();
            Assert.IsNotEmpty(lUnreadMessages);
            Assert.AreEqual(3, lUnreadMessages.Length);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            this._conferenceInfo =
                (await Conference.GetConferences(GeneralSetup.SenpaiInstance).ThrowFirstForNonSuccess()).First
                    ();
            Assert.IsNotNull(this._conferenceInfo);
        }

        #endregion
    }
}