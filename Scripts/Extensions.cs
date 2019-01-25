using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using SolAR.Datastructure;
using UnityEngine;
using UnityEngine.Assertions;
using XPCF.Api;
using XPCF.Core;
using XPCF.Properties;

#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    public static class Extensions
    {
        /*
        static void CalibCamera(Camera m_camera, PipelineManager.CamParams camParams)
        {
                    var m_texture = new Texture2D(camParams.width, camParams.height, TextureFormat.RGB24, false);
                    m_texture.filterMode = FilterMode.Point;
                    m_texture.Apply();

                    m_material.mainTexture = m_texture;
                    m_canvas.transform.GetChild(0).GetComponent<RawImage>().material = m_material;
                    m_canvas.transform.GetChild(0).GetComponent<RawImage>().texture = m_texture;

            // Set Camera projection matrix according to calibration parameters provided by SolAR Pipeline
            Matrix4x4 projectionMatrix = new Matrix4x4();
            float near = m_camera.nearClipPlane;
            float far = m_camera.farClipPlane;

            Vector4 row0 = new Vector4(2.0f * camParams.focalX / camParams.width, 0, 1.0f - 2.0f * camParams.centerX / camParams.width, 0);
            Vector4 row1 = new Vector4(0, 2.0f * camParams.focalY / camParams.height, 2.0f * camParams.centerY / camParams.height - 1.0f, 0);
            Vector4 row2 = new Vector4(0, 0, (far + near) / (near - far), 2.0f * far * near / (near - far));
            Vector4 row3 = new Vector4(0, 0, -1, 0);

            projectionMatrix.SetRow(0, row0);
            projectionMatrix.SetRow(1, row1);
            projectionMatrix.SetRow(2, row2);
            projectionMatrix.SetRow(3, row3);

            m_camera.fieldOfView = (Mathf.Rad2Deg * 2 * Mathf.Atan(camParams.width / (2 * camParams.focalX))) - 10;
            m_camera.projectionMatrix = projectionMatrix;

            //m_eventCallback = new eventCallbackDelegate(m_pipelineManager.updateFrameDataOGL);
        }
        */

        static Matrix4x4 invertMatrix;
        static Extensions()
        {
            invertMatrix = new Matrix4x4();
            invertMatrix.SetRow(0, new Vector4(+1, +0, +0, +0));
            invertMatrix.SetRow(1, new Vector4(+0, -1, +0, +0));
            invertMatrix.SetRow(2, new Vector4(+0, +0, +1, +0));
            invertMatrix.SetRow(3, new Vector4(+0, +0, +0, +1));
        }

        static Pose CallPluginAtEndOfFrames(Transform3Df pose)
        {
            Matrix4x4 cameraPoseFromSolAR = new Matrix4x4();

            var pos = pose.translation();
            var rot = pose.rotation();
            cameraPoseFromSolAR.SetRow(0, new Vector4(rot.coeff(0, 0), rot.coeff(0, 1), rot.coeff(0, 2), pos.coeff(0, 0)));
            cameraPoseFromSolAR.SetRow(1, new Vector4(rot.coeff(1, 0), rot.coeff(1, 1), rot.coeff(1, 2), pos.coeff(0, 1)));
            cameraPoseFromSolAR.SetRow(2, new Vector4(rot.coeff(2, 0), rot.coeff(2, 1), rot.coeff(2, 2), pos.coeff(0, 2)));
            cameraPoseFromSolAR.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 unityCameraPose = invertMatrix * cameraPoseFromSolAR;

            Vector3 forward = new Vector3(unityCameraPose.m02, unityCameraPose.m12, unityCameraPose.m22);
            Vector3 up = new Vector3(unityCameraPose.m01, unityCameraPose.m11, unityCameraPose.m21);

            var q = Quaternion.LookRotation(forward, -up);
            var p = new Vector3(unityCameraPose.m03, unityCameraPose.m13, unityCameraPose.m23);
            return new Pose(p, q);
        }

        public static void ToUnity(this Image image, ref Texture2D texture)
        {
            var w = (int)image.getWidth();
            var h = (int)image.getHeight();
            TextureFormat format;
            switch (image.getImageLayout())
            {
                case Image.ImageLayout.LAYOUT_RGB:
                case Image.ImageLayout.LAYOUT_GRB:
                case Image.ImageLayout.LAYOUT_BGR:
                    format = TextureFormat.RGB24;
                    break;
                case Image.ImageLayout.LAYOUT_GREY:
                    format = TextureFormat.Alpha8;
                    break;
                case Image.ImageLayout.LAYOUT_RGBA:
                case Image.ImageLayout.LAYOUT_RGBX:
                case Image.ImageLayout.LAYOUT_UNDEFINED:
                default:
                    format = TextureFormat.RGBA32;
                    break;
            }
            Assert.AreEqual(3, image.getNbChannels());
            Assert.AreEqual(8, image.getNbBitsPerComponent());
            Assert.AreEqual(Image.DataType.TYPE_8U, image.getDataType());
            //Assert.AreEqual(Image.ImageLayout.LAYOUT_BGR, image.getImageLayout());
            Assert.AreEqual(Image.PixelOrder.INTERLEAVED, image.getPixelOrder());
            if (texture != null && (texture.width != w || texture.height != h || texture.format != format))
            {
                UnityEngine.Object.Destroy(texture);
                texture = null;
            }
            if (texture == null)
            {
                texture = new Texture2D(w, h, format, false);
            }
            texture.LoadRawTextureData(image.data(), (int)image.getBufferSize());
            texture.Apply();
        }

        public static Image ToSolAR(this Texture2D texture)
        {
            var data = texture.GetRawTextureData();
            var handler = GCHandle.Alloc(data, GCHandleType.Pinned);
            var ptr = handler.AddrOfPinnedObject();

            uint w = (uint)texture.width;
            uint h = (uint)texture.height;
            Image.ImageLayout pixLayout;
            switch (texture.format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.R16:
                case TextureFormat.R8:
                case TextureFormat.RFloat:
                case TextureFormat.RHalf:
                    pixLayout = Image.ImageLayout.LAYOUT_GREY;
                    break;
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                case TextureFormat.RGBA32:
                case TextureFormat.RGBAFloat:
                case TextureFormat.RGBAHalf:
                    pixLayout = Image.ImageLayout.LAYOUT_RGBA;
                    break;
                case TextureFormat.RG16:
                case TextureFormat.RGFloat:
                case TextureFormat.RGHalf:
                    pixLayout = Image.ImageLayout.LAYOUT_UNDEFINED;
                    break;
                case TextureFormat.RGB24:
                    pixLayout = Image.ImageLayout.LAYOUT_RGB;
                    break;
                default:
                    Debug.LogWarning(new { texture.format });
                    pixLayout = Image.ImageLayout.LAYOUT_UNDEFINED;
                    break;
            }
            Image.DataType type;
            switch (texture.format)
            {
                case TextureFormat.R16:
                case TextureFormat.RG16:
                    type = Image.DataType.TYPE_16U;
                    break;
                case TextureFormat.RFloat:
                case TextureFormat.RGBAFloat:
                case TextureFormat.RGFloat:
                    type = Image.DataType.TYPE_32U;
                    break;
                case TextureFormat.Alpha8:
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                case TextureFormat.R8:
                case TextureFormat.RGB24:
                case TextureFormat.RGBA32:
                    type = Image.DataType.TYPE_8U;
                    break;
                case TextureFormat.RGBAHalf:
                case TextureFormat.RGHalf:
                case TextureFormat.RHalf:
                    type = Image.DataType.TYPE_16U;
                    break;
                default:
                    Debug.LogWarning(new { texture.format });
                    type = Image.DataType.TYPE_8U;
                    break;
            }
            var image = new Image(ptr, w, h, pixLayout, Image.PixelOrder.INTERLEAVED, type);
            handler.Free();
            return image;
        }

        public static Pose ToUnity(this Transform3Df pose)
        {
            var inv = new Matrix4x4();
            inv.SetRow(0, new Vector4(+1, +0, +0, +0));
            inv.SetRow(1, new Vector4(+0, -1, +0, +0));
            inv.SetRow(2, new Vector4(+0, +0, +1, +0));
            inv.SetRow(3, new Vector4(+0, +0, +0, +1));

            var rot = pose.rotation();
            var trans = pose.translation();
            var m = new Matrix4x4();
            for (int r = 0; r < 3; ++r)
            {
                for (int c = 0; c < 3; ++c)
                {
                    m[r, c] = rot.coeff(r, c);
                }
                m[r, 3] = trans.coeff(r, 0);
            }
            m.SetRow(3, new Vector4(0, 0, 0, 1));

            m = m.inverse;

            m = inv * m;

            //m = m.inverse;

            var v = m.GetColumn(3);
            var q = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));

            q = Quaternion.Inverse(q);
            v = q * -v;

            return new Pose(v, q);
        }

        public static bool CanRead(this IProperty.AccessSpecifier specifier) { return specifier != IProperty.AccessSpecifier.IProperty_OUT; }
        public static bool CanWrite(this IProperty.AccessSpecifier specifier) { return specifier != IProperty.AccessSpecifier.IProperty_IN; }

        public static object Get(this IProperty property, uint itemIndex = 0)
        {
            switch (property.getType())
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                    return property.getStringValue(itemIndex);
                case IProperty.PropertyType.IProperty_DOUBLE:
                    return property.getDoubleValue(itemIndex);
                case IProperty.PropertyType.IProperty_FLOAT:
                    return property.getFloatingValue(itemIndex);
                case IProperty.PropertyType.IProperty_INTEGER:
                    return property.getIntegerValue(itemIndex);
                case IProperty.PropertyType.IProperty_LONG:
                    return property.getLongValue(itemIndex);
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    return property.getStructureValue(itemIndex);
                case IProperty.PropertyType.IProperty_UINTEGER:
                    return property.getUnsignedIntegerValue(itemIndex);
                case IProperty.PropertyType.IProperty_ULONG:
                    return property.getUnsignedLongValue(itemIndex);
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    return property.getUnicodeStringValue(itemIndex);
                default:
                    throw new System.Exception();
            }
        }

        public static void Set(this IProperty property, object value, uint itemIndex = 0)
        {
            switch (property.getType())
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                    property.setStringValue((string)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_DOUBLE:
                    property.setDoubleValue((double)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_FLOAT:
                    property.setFloatingValue((float)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_INTEGER:
                    property.setIntegerValue((int)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_LONG:
                    property.setLongValue((long)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    property.setStructureValue((IPropertyMap)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_UINTEGER:
                    property.setUnsignedIntegerValue((uint)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_ULONG:
                    property.setUnsignedLongValue((ulong)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    property.setUnicodeStringValue((string)value, itemIndex);
                    break;
                default:
                    throw new System.Exception();
            }
        }

        public static object Default(this IProperty.PropertyType propertyType)
        {
            switch (propertyType)
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    return default(string);
                case IProperty.PropertyType.IProperty_DOUBLE:
                    return default(double);
                case IProperty.PropertyType.IProperty_FLOAT:
                    return default(float);
                case IProperty.PropertyType.IProperty_INTEGER:
                    return default(int);
                case IProperty.PropertyType.IProperty_LONG:
                    return default(long);
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    return default(IPropertyMap);
                case IProperty.PropertyType.IProperty_UINTEGER:
                    return default(uint);
                case IProperty.PropertyType.IProperty_ULONG:
                    return default(ulong);
                default:
                    throw new System.Exception();
            }
        }

        public static object OnGUI(this IProperty.PropertyType propertyType, object value)
        {
            switch (propertyType)
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    return GUILayout.TextField((string)value);
                case IProperty.PropertyType.IProperty_DOUBLE:
                    {
                        var v = (double)value;
                        using (Scope.ChangeCheck)
                        {
                            var text = GUILayout.TextField(value.ToString());
                            if (GUI.changed)
                            {
                                if (double.TryParse(text, out v))
                                {
                                    value = v;
                                }
                            }
                        }
                    }
                    return value;
                case IProperty.PropertyType.IProperty_FLOAT:
                    {
                        var v = (float)value;
                        using (Scope.ChangeCheck)
                        {
                            var text = GUILayout.TextField(value.ToString());
                            if (GUI.changed)
                            {
                                if (float.TryParse(text, out v))
                                {
                                    value = v;
                                }
                            }
                        }
                    }
                    return value;
                case IProperty.PropertyType.IProperty_INTEGER:
                    {
                        var v = (int)value;
                        using (Scope.ChangeCheck)
                        {
                            var text = GUILayout.TextField(value.ToString());
                            if (GUI.changed)
                            {
                                if (int.TryParse(text, out v))
                                {
                                    value = v;
                                }
                            }
                        }
                    }
                    return value;
                case IProperty.PropertyType.IProperty_LONG:
                    {
                        var v = (long)value;
                        using (Scope.ChangeCheck)
                        {
                            var text = GUILayout.TextField(value.ToString());
                            if (GUI.changed)
                            {
                                if (long.TryParse(text, out v))
                                {
                                    value = v;
                                }
                            }
                        }
                    }
                    return value;
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    return default(IPropertyMap);
                case IProperty.PropertyType.IProperty_UINTEGER:
                    {
                        var v = (uint)value;
                        using (Scope.ChangeCheck)
                        {
                            var text = GUILayout.TextField(value.ToString());
                            if (GUI.changed)
                            {
                                if (uint.TryParse(text, out v))
                                {
                                    value = v;
                                }
                            }
                        }
                    }
                    return value;
                case IProperty.PropertyType.IProperty_ULONG:
                    {
                        var v = (ulong)value;
                        using (Scope.ChangeCheck)
                        {
                            var text = GUILayout.TextField(value.ToString());
                            if (GUI.changed)
                            {
                                if (ulong.TryParse(text, out v))
                                {
                                    value = v;
                                }
                            }
                        }
                    }
                    return value;
                default:
                    GUILayout.TextField(value.ToString());
                    return value;
            }
        }

        public static readonly Dictionary<string, string> modulesDict = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> interfacesDict = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> componentsDict = new Dictionary<string, string>();

        public static string GetUUID(string name)
        {
            string res = null;
            bool ok = false;
            ok = ok || modulesDict.TryGetValue(name, out res);
            ok = ok || interfacesDict.TryGetValue(name, out res);
            ok = ok || componentsDict.TryGetValue(name, out res);
            if (!ok) throw new System.Exception("Unknown UUID for: " + name);
            return res;
        }

        public static UUID GET_UUID<T>()
        {
            var name = typeof(T).Name;
            return new UUID(GetUUID(name));
        }

        [Obsolete("Use type parameter")]
        public static IComponentIntrospect create<T>(this IComponentManager xpcfComponentManager)
        {
            return xpcfComponentManager.createComponent(GET_UUID<T>());
        }

        public static IComponentIntrospect create(this IComponentManager xpcfComponentManager, string type)
        {
            var uuid = new UUID(GetUUID(type));
            return xpcfComponentManager.createComponent(uuid);
        }

        public static IComponentIntrospect create(this IComponentManager xpcfComponentManager, string type, string name)
        {
            var uuid = new UUID(GetUUID(type));
            return xpcfComponentManager.createComponent(name, uuid);
        }

        public static T bindTo<T>(this IComponentIntrospect component) where T : class
        {
            return (T)component.bindTo(typeof(T).Name);
        }

        public static object bindTo(this IComponentIntrospect component, string name)
        {
            var type = typeof(solar);
            var method = type.GetMethod("bindTo_" + name, BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(method);
            return method.Invoke(null, new[] { component });
        }

        [Obsolete]
        public static T CastTo<T>(IComponentIntrospect from, bool cMemoryOwn = false)
        {
            var type = typeof(IComponentIntrospect);
            MethodInfo getCPtr = type.GetMethod("getCPtr", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(getCPtr);
            FieldInfo swigCMemOwnBase = type.GetField("swigCMemOwnBase", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(swigCMemOwnBase);
            //cMemoryOwn = (bool) swigCMemOwnBase.GetValue(from);
            swigCMemOwnBase.SetValue(from, false);
            var cptr = (HandleRef)getCPtr.Invoke(null, new object[] { from });
            return (T)Activator.CreateInstance
            (
                typeof(T),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] { cptr.Handle, cMemoryOwn },
                null
            );
        }
    }
}
#pragma warning restore IDE1006 // Styles d'affectation de noms
