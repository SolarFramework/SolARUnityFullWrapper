﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using SolAR.Core;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    public abstract class AbstractSample : MonoBehaviour
    {
        protected readonly IList<IDisposable> subscriptions = new List<IDisposable>();

        protected FrameworkReturnCode ok { set { Assert.AreEqual(FrameworkReturnCode._SUCCESS, value); } }

        [HideInInspector]
        public Configuration conf;
        //public ConfigurationSO confSO;

        [ContextMenu("Load")]
        void Load()
        {
            var serializer = new XmlSerializer(typeof(ConfXml));
            using (var stream = File.Open(conf.path, FileMode.Open))
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
                    Encoding = System.Text.Encoding.UTF8,
                };
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                */
                {
                    serializer.Serialize(writer, conf.conf, namespaces);
                }
                Debug.Log(writer.ToString());
            }
        }

        [ContextMenu("Save")]
        void Save()
        {
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });
            var serializer = new XmlSerializer(typeof(ConfXml));
            using (var stream = File.Open(conf.path, FileMode.Create))
            {
                serializer.Serialize(stream, conf.conf, namespaces);
            }
        }

        protected Texture2D inputTex;

        protected virtual void OnEnable()
        {
            foreach (var kvp in conf.conf.modules.ToDictionary(m => m.name, m => m.uuid))
            {
                Extensions.modulesDict[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in conf.conf.modules.SelectMany(m => m.components).ToDictionary(c => c.name, c => c.uuid))
            {
                Extensions.componentsDict[kvp.Key] = kvp.Value;
            }
            var comparer = new KeyBasedEqualityComparer<ConfXml.Module.Component.Interface, string>(i => i.uuid);
            foreach (var kvp in conf.conf.modules.SelectMany(m => m.components).SelectMany(c => c.interfaces).Distinct(comparer).ToDictionary(i => i.name, i => i.uuid))
            {
                Extensions.interfacesDict[kvp.Key] = kvp.Value;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var d in subscriptions) d.Dispose();
            subscriptions.Clear();
        }

        protected void printf(string format, params object[] objs) { Debug.LogFormat(format, objs); }

        protected void LOG_ERROR(string message, params object[] objects) { Debug.LogErrorFormat(this, message, objects); }
        protected void LOG_INFO(string message, params object[] objects) { Debug.LogWarningFormat(this, message, objects); }
        protected void LOG_DEBUG(string message, params object[] objects) { Debug.LogFormat(this, message, objects); }

        protected const long CLOCKS_PER_SEC = TimeSpan.TicksPerSecond;

        protected static long clock() { return DateTimeOffset.Now.Ticks; }
    }
}
#pragma warning restore IDE1006 // Styles d'affectation de noms
