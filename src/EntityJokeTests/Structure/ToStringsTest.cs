﻿using EntityJoke.Core;
using EntityJoke.Structure;
using EntityJokeTests.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityJokeTests.Structure
{
    [TestFixture]
    public class ToStringsTest
    {
        Entity entity;

        [SetUp]
        public void SetUp()
        {
            DictionaryEntitiesMap.INSTANCE.TryAddEntity(typeof(Produto));
            entity = DictionaryEntitiesMap.INSTANCE.GetEntity(typeof(Produto));
        }

        [Test]
        public void AssertEntityToString()
        {
            Assert.That(entity.ToString(), Is.EqualTo("produto: EntityJokeTests.Core.Produto"));
        }

        [Test]
        public void AssertFieldToString()
        {
            Assert.That(entity.GetFields()[0].ToString(), Is.EqualTo("[id]Id: System.Int32"));
        }

        [Test]
        public void AssertAliasToString()
        {
            Alias alias = new Alias(entity, "p");
            Assert.That(alias.ToString(), Is.EqualTo("Produto: p"));
        }

    }
}
