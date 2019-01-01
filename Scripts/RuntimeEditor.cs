using System.Collections.Generic;
using System.Linq;
using SolAR;
using UnityEngine;
using XPCF;

public partial class RuntimeEditor : MonoBehaviour
{
    static readonly UUID configurableUUID = UUID.Create("98DBA14F-6EF9-462E-A387-34756B4CBA80");

    public IComponentManager xpcfManager { get { return xpcf.getComponentManagerInstance(); } }

    readonly IList<IComponentIntrospect> xpcfComponents = new List<IComponentIntrospect>();
    GUIContent[] guiComponents;
    int idComponent = -1;
    IComponentIntrospect xpcfComponent;

    //UUID[] xpcfInterfaces;
    //GUIContent[] guiInterfaces;
    //int idInterface = -1;

    IConfigurable xpcfConfigurable;

    public void Add(IComponentIntrospect component)
    {
        xpcfComponents.Add(component);
        guiComponents = xpcfComponents.Select(c => new GUIContent(c.GetType().Name)).ToArray();
    }

    protected void OnGUI()
    {
        using (new GUILayout.HorizontalScope())
        {
            if (guiComponents != null)
            {
                using (GUITools.ChangeCheckScope)
                {
                    idComponent = GUILayout.SelectionGrid(idComponent, guiComponents, 1, GUILayout.Width(300));
                    if (GUI.changed)
                    {
                        xpcfComponent = xpcfComponents[idComponent];

                        /*
                        foreach (var uuid in xpcfComponent.getInterfaces())
                        {
                            Debug.Log(xpcfComponent.getDescription(uuid));
                            //var metadata = xpcfManager.findInterfaceMetadata(uuid);
                            //Debug.Log(metadata.name());
                            //Debug.Log(metadata.description());
                        }
                        */
                        /*
                        xpcfInterfaces = xpcfComponent.getInterfaces().ToArray();
                        guiInterfaces = xpcfInterfaces
                            .Select(xpcfComponent.getDescription)
                            .Select(s => new GUIContent("", s))
                            .ToArray();
                            */

                        xpcfConfigurable = xpcfComponent.implements(configurableUUID) ? xpcfComponent.bindTo<IConfigurable>() : null;
                    }
                }
            }
            /*
            if (guiInterfaces != null)
            {
                using (GUITools.ChangeCheckScope)
                {
                    idInterface = GUILayout.SelectionGrid(idInterface, guiInterfaces, 1);
                    if (GUI.changed)
                    {
                        var uuid = xpcfInterfaces[idInterface];

                        //var xpcfInstance = xpcfComponent.bindTo("");
                        guiInterfaces = xpcfComponent.getInterfaces()
                            .Select(xpcfComponent.getDescription)
                            .Select(s => new GUIContent("", s))
                            .ToArray();
                    }
                }
            }
            */
            if (xpcfConfigurable == null)
            {
                GUILayout.Label("This component is not IConfigurable");
            }
            else
            {
                using (new GUILayout.VerticalScope())
                {
                    foreach (var p in xpcfConfigurable.getProperties())
                    {
                        var access = p.getAccessSpecifier();
                        var type = p.getType();
                        object value = access.CanRead() ? value = p.Get() : type.Default();

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label(p.getName(), GUILayout.Width(300));
                            var size = p.size();
                            using (GUITools.ChangeCheckScope)
                            {
                                value = type.OnGUI(value);
                                if (access.CanWrite() && GUI.changed)
                                {
                                    p.Set(value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
