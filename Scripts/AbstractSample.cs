using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SolAR.Core;
using UnityEngine;

#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    public abstract class AbstractSample : MonoBehaviour
    {
        protected FrameworkReturnCode ok;

        protected readonly IList<IDisposable> subscriptions = new List<IDisposable>();

        protected virtual void OnDisable()
        {
            foreach (var d in subscriptions) d.Dispose();
            subscriptions.Clear();
        }

        [HideInInspector]
        public Configuration conf;
        //public ConfigurationSO confSO;

        [ContextMenu("Load")]
        void Load()
        {
            var serializer = new XmlSerializer(typeof(ConfXml));
            using (var stream = File.OpenRead(conf.path))
            {
                conf.conf = (ConfXml)serializer.Deserialize(stream);
            }
        }

        [ContextMenu("Print")]
        void Print()
        {
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });
            var serializer = new XmlSerializer(typeof(ConfXml));

            using (var writer = new StringWriter())
            {
                /*
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                };
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                */
                {
                    serializer.Serialize(writer, conf.conf, namespaces);
                    Debug.Log(writer.ToString());
                }
            }
        }

        public string[] argv;
        int argc { get { return argv.Length; } }

        protected void printf(string format, params object[] objs) { Debug.LogFormat(format, objs); }

        protected void LOG_ADD_LOG_TO_CONSOLE() { }

        protected static void LOG_ERROR(string message) { Debug.LogError(message); }
        protected static void LOG_INFO(string message) { Debug.LogWarning(message); }
        protected static void LOG_DEBUG(string message, params object[] objects) { Debug.LogFormat(message, objects); }

        protected const long CLOCKS_PER_SEC = TimeSpan.TicksPerSecond;

        protected static long clock() { return DateTimeOffset.Now.Ticks; }
    }
}
#pragma warning restore IDE1006 // Styles d'affectation de noms
