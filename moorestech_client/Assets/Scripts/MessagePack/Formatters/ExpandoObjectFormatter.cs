﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Dynamic;

namespace MessagePack.Formatters
{
    public class ExpandoObjectFormatter : IMessagePackFormatter<ExpandoObject>
    {
        public static readonly IMessagePackFormatter<ExpandoObject> Instance = new ExpandoObjectFormatter();

        private ExpandoObjectFormatter()
        {
        }

        public ExpandoObject Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil()) return null;

            var result = new ExpandoObject();
            var count = reader.ReadMapHeader();
            if (count > 0)
            {
                var resolver = options.Resolver;
                var keyFormatter = resolver.GetFormatterWithVerify<string>();
                var valueFormatter = resolver.GetFormatterWithVerify<object>();
                IDictionary<string, object> dictionary = result;

                options.Security.DepthStep(ref reader);
                try
                {
                    for (var i = 0; i < count; i++)
                    {
                        var key = keyFormatter.Deserialize(ref reader, options);
                        var value = valueFormatter.Deserialize(ref reader, options);
                        dictionary.Add(key, value);
                    }
                }
                finally
                {
                    reader.Depth--;
                }
            }

            return result;
        }

        public void Serialize(ref MessagePackWriter writer, ExpandoObject value, MessagePackSerializerOptions options)
        {
            var dict = (IDictionary<string, object>)value;
            var keyFormatter = options.Resolver.GetFormatterWithVerify<string>();
            var valueFormatter = options.Resolver.GetFormatterWithVerify<object>();

            writer.WriteMapHeader(dict.Count);
            foreach (var item in dict)
            {
                keyFormatter.Serialize(ref writer, item.Key, options);
                valueFormatter.Serialize(ref writer, item.Value, options);
            }
        }
    }
}