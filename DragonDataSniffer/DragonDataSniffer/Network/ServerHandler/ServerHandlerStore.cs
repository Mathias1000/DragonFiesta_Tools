﻿using DragonDataSniffer.Network;
using DragonDataSniffer.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DragonDataSniffer.ServerHandler
{
    [TunnelModule(InitializationStage.Metadata)]
    public class ServerHandlerStore
    {
        private static Dictionary<byte, Dictionary<byte, MethodInfo>> handlers;

        [InitializerMethod]
        public static bool Load()
        {
            handlers = new Dictionary<byte, Dictionary<byte, MethodInfo>>();
            foreach (var info in Reflector.FindMethodsByAttribute<ServerPacketHandlerAttribute>())
            {
                ServerPacketHandlerAttribute attribute = info.First;
                MethodInfo method = info.Second;
                if (!handlers.ContainsKey(attribute.Header))
                    handlers.Add(attribute.Header, new Dictionary<byte, MethodInfo>());
                if (handlers[attribute.Header].ContainsKey(attribute.Type))
                {
                    Log.WriteLine(LogLevel.Warn, "Duplicate Serverhandler found: {0}:{1}", attribute.Header, attribute.Type);
                    handlers[attribute.Header].Remove(attribute.Type);
                }
                handlers[attribute.Header].Add(attribute.Type, method);
            }

            int count = 0;
            foreach (var dict in handlers.Values)
                count += dict.Count;
            Log.WriteLine(LogLevel.Info, "{0} ServerHandlers loaded.", count);
            return true;
        }

        public static MethodInfo GetHandler(byte header, byte type)
        {
            if (handlers.TryGetValue(header, out Dictionary<byte, MethodInfo> dict))
            {
                if (dict.TryGetValue(type, out MethodInfo meth))
                {
                    return meth;
                }
            }
            return null;
        }

        public static Action GetCallback(MethodInfo method, params object[] parameters)
        {
            return () => method.Invoke(null, parameters);
        }
    }
}