﻿// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Datasync.Common.Test;
using Datasync.Common.Test.Models;
using Microsoft.Datasync.Client.Serialization;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Microsoft.Datasync.Client.Test.Serialization
{
    [ExcludeFromCodeCoverage]
    public class DatasyncContractResolver_Tests : BaseTest
    {
        private readonly WrappedDatasyncContractResolver contractResolver;

        public DatasyncContractResolver_Tests()
        {
            contractResolver = new WrappedDatasyncContractResolver();
        }

        [Fact]
        [Trait("Method", "CamelCasePropertyNames")]
        public void CamelCasePropertyNames_Roundtrips()
        {
            contractResolver.CamelCasePropertyNames = true;
            Assert.True(contractResolver.CamelCasePropertyNames);

            contractResolver.CamelCasePropertyNames = false;
            Assert.False(contractResolver.CamelCasePropertyNames);
        }

        [Theory]
        [InlineData(typeof(PocoType), "pocotype")]
        [InlineData(typeof(DataTableType), "nameddatatabletype")]
        [InlineData(typeof(JsonContainerType), "namedjsoncontainertype")]
        [InlineData(typeof(UnnamedJsonContainerType), "unnamedjsoncontainertype")]
        [Trait("Method", "ResolveTableName")]
        public void ResolveTableName_Tests(Type sut, string expected)
        {
            var actual = contractResolver.ResolveTableName(sut);
            Assert.Equal(expected, actual);
        }

        [Fact]
        [Trait("Method", "ResolveTableName")]
        public void ResolveTableName_Throws_WithoutId()
        {
            Assert.Throws<InvalidOperationException>(() => contractResolver.ResolveTableName(typeof(NoIdEntity)));
        }

        [Fact]
        [Trait("Method", "ResolveTableName")]
        public void ResolveTableName_Throws_MultiId()
        {
            Assert.Throws<InvalidOperationException>(() => contractResolver.ResolveTableName(typeof(MultiIdType)));
        }

        [Theory]
        [InlineData(false, null, null)]
        [InlineData(false, "", "")]
        [InlineData(false, "a", "a")]
        [InlineData(false, "A", "A")]
        [InlineData(false, "version", "version")]
        [InlineData(false, "Version", "Version")]
        [InlineData(false, "updatedAt", "updatedAt")]
        [InlineData(false, "UpdatedAt", "UpdatedAt")]
        [InlineData(true, null, null)]
        [InlineData(true, "", "")]
        [InlineData(true, "a", "a")]
        [InlineData(true, "A", "a")]
        [InlineData(true, "version", "version")]
        [InlineData(true, "Version", "version")]
        [InlineData(true, "updatedAt", "updatedAt")]
        [InlineData(true, "UpdatedAt", "updatedAt")]
        [Trait("Method", "ResolvePropertyName")]
        public void ResolvePropertyName_Tests(bool camelCase, string sut, string expected)
        {
            contractResolver.CamelCasePropertyNames = camelCase;
            string actual = contractResolver.P_ResolvePropertyName(sut);
            Assert.Equal(expected, actual);
        }

        #region Test Models
        [DataTable("nameddatatabletype")]
        public class DataTableType
        {
            public string Id { get; set; }
        }

        public class PocoType
        {
            public string Id { get; set; }

            [Version]
            public string JsonVersion { get; set; }

            [UpdatedAt]
            public DateTimeOffset? JsonUpdatedAt { get; set; }
        }

        public class MultiId
        {
            public string Id { get; set; }

            [JsonProperty("id")]
            public string JsonId { get; set; }
        }

        [JsonObject(Title = "namedjsoncontainertype")]
        public class JsonContainerType
        {
            public string Id { get; set; }
        }

        [JsonObject]
        public class UnnamedJsonContainerType
        {
            public string Id { get; set; }
        }

        public class MultiIdType
        {
            public string ID { get; set; }
            public string Id { get; set; }
        }

        public class WrappedDatasyncContractResolver : DatasyncContractResolver
        {
            internal string P_ResolvePropertyName(string propertyName) => base.ResolvePropertyName(propertyName);
        }
        #endregion
    }
}
