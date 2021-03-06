﻿using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationIdTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_id_not_on_root_serialize_without_id()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Title = "My title"
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""attributes"": {
                        title: ""My title""
                    }
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_int_should_serialize_as_string()
        {
            var root = DocumentRoot.Create(new
            {
                Id = 7357, //int id
                Type = "articles",
                Title = "My title",
                Author = new
                {
                    Id = 7357, //int relationship
                    Type = "person",
                    Name = "Scribble McPen"
                }
            });

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
              ""data"": {
                ""id"": ""7357"",
                ""type"": ""articles"",
                ""attributes"": {
                  ""title"": ""My title""
                },
                ""relationships"": {
                  ""author"": {
                    ""data"": {
                      ""id"": ""7357"",
                      ""type"": ""person""
                    }
                  }
                }
              },
              ""included"": [
                {
                  ""id"": ""7357"",
                  ""type"": ""person"",
                  ""attributes"": {
                    ""name"": ""Scribble McPen""
                  }
                }
              ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_int_in_explicit_identifier_should_serialize_as_string()
        {
            var root = DocumentRoot.Create(new
            {
                Id = 7357, //int id
                Type = "articles",
                Title = "My title",
                Author = ResourceIdentifier.Create(new
                {
                    Id = 7357, //int relationship
                    Type = "person",
                    Name = "Scribble McPen"
                })
            });

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
              ""data"": {
                ""id"": ""7357"",
                ""type"": ""articles"",
                ""attributes"": {
                  ""title"": ""My title""
                },
                ""relationships"": {
                  ""author"": {
                    ""data"": {
                      ""id"": ""7357"",
                      ""type"": ""person""
                    }
                  }
                }
              },
              ""included"": [
                {
                  ""id"": ""7357"",
                  ""type"": ""person"",
                  ""attributes"": {
                    ""name"": ""Scribble McPen""
                  }
                }
              ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_null_should_serialize_as_string()
        {
            var root = DocumentRoot.Create(new
            {
                Id = (int?)null,
                Type = "articles",
                Title = "My title",
                Author = ResourceIdentifier.Create(new
                {
                    Id = 7357, //int relationship
                    Type = "person",
                    Name = "Scribble McPen"
                })
            });

            var json = JsonConvert.SerializeObject(root, new JsonApiSerializerSettings() {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented
            });
            var expectedjson = @"{
              ""data"": {
                ""id"": null,
                ""type"": ""articles"",
                ""attributes"": {
                  ""title"": ""My title""
                },
                ""relationships"": {
                  ""author"": {
                    ""data"": {
                      ""id"": ""7357"",
                      ""type"": ""person""
                    }
                  }
                }
              },
              ""included"": [
                {
                  ""id"": ""7357"",
                  ""type"": ""person"",
                  ""attributes"": {
                    ""name"": ""Scribble McPen""
                  }
                }
              ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_object_should_throw()
        {
            var root = new DocumentRoot<ArticleWithIdType<Tuple<string,string>>>
            {
                Data = new ArticleWithIdType<Tuple<string, string>>
                {
                    Id = new Tuple<string, string>("1", "2"),
                    Title = "My title"
                }
            };

            var ex = Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, settings));
        }

        [Fact]
        public void When_id_null_object_should_throw()
        {
            var root = new DocumentRoot<ArticleWithIdType<Tuple<string, string>>>
            {
                Data = new ArticleWithIdType<Tuple<string, string>>
                {
                    Id = (Tuple<string, string>)null,
                    Title = "My title"
                }
            };

            var ex = Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, settings));
        }

        [Fact]
        public void When_id_invalid_type_and_null_and_show_nulls_should_throw()
        {
            var root = new DocumentRoot<ArticleWithIdType<Task>>
            {
                Data = new ArticleWithIdType<Task>
                {
                    Id = null,
                    Title = "My title"
                }
            };

            var showNullSettings = new JsonApiSerializerSettings { NullValueHandling = NullValueHandling.Include };
            Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, showNullSettings));
        }
    }
}
