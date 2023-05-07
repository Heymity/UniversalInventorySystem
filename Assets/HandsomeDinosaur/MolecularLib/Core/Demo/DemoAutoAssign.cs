using System.Collections.Generic;
using System.Text;
using MolecularLib.AutoAssign;
using UnityEngine;

namespace MolecularLib.Demo
{
    // Remember to add this attribute here on the class and call this.AutoAssign()
    // if you don't derive it from AutoAssignMonoBehaviour!
    //[UseAutoAssign]
    public class DemoAutoAssign : AutoAssignMonoBehaviour
    {
        [GetComponent] private Rigidbody2D _rigidbody2D;
        [GetComponent] private Rigidbody2D Rigidbody2DProp { get; set; }

        [GetComponents(typeof(BoxCollider2D))] private List<Collider2D> _colliders;
        [GetComponents(typeof(BoxCollider2D))] private List<Collider2D> CollidersProp { get; set; }
        
        [GetComponentInChildren] private Collider2D _collider;
        [GetComponentInChildren] private Collider2D ColliderProp { get; set; }
        
        [GetComponentsInChildren] private List<Transform> _transformList;
        [GetComponentsInChildren] private List<Transform> TransformListProp { get; set; }
        
        [Find("DEMO DRAWERS")] private GameObject _drawers;
        [Find("DEMO DRAWERS")] private GameObject DrawersProp { get; set; }
        
        [FindWithTag("MainCamera")] private GameObject _camera;
        [FindWithTag("MainCamera")] private GameObject CameraProp { get; set; }
        
        [FindGameObjectsWithTag("GameController")] private GameObject[] _gameControllers;
        [FindGameObjectsWithTag("GameController")] private GameObject[] GameControllersProp { get; set; }
        
        [FindObjectOfType(typeof(DemoDrawersScript))] private DemoDrawersScript _drawersScript;
        [FindObjectOfType(typeof(DemoDrawersScript))] private DemoDrawersScript DrawersScriptProp { get; set; }
        
        [FindObjectsOfType(typeof(DemoDrawersScript))] private DemoDrawersScript[] _drawersScripts;
        [FindObjectsOfType(typeof(DemoDrawersScript))] private DemoDrawersScript[] DrawersScriptsProp { get; set; }
        
        [LoadResource("ArgsInstantiated")] private GameObject _gameObject;
        [LoadResource("ArgsInstantiated")] private GameObject GameObjectProp { get; set; } 
        

        /* If you can't derive from AutoAssignMonoBehaviour, you can just call the function below like that
        private void Awake()
        {
            this.AutoAssign();
        }*/
        
        
        
        [ContextMenu("Test")]
        public void Test()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Demo AutoAssign:");
            builder.AppendLine($"GetComponent: Field {_rigidbody2D} | Prop {Rigidbody2DProp}");
            builder.AppendLine($"GetComponents: Field {_colliders} ({_colliders.Count}) | Prop {CollidersProp} ({CollidersProp.Count})");
            builder.AppendLine($"GetComponentInChildren: Field {_collider} | Prop {ColliderProp}");
            builder.AppendLine($"GetComponentsInChildren: Field {_transformList} ({_transformList.Count}) | Prop {TransformListProp} ({TransformListProp.Count})");
            builder.AppendLine($"Find: Field {_drawers} | Prop {DrawersProp}");
            builder.AppendLine($"FindWithTag: Field {_camera} | Prop {CameraProp}");
            builder.AppendLine(
                $"FindGameObjectsWithTag: Field {_gameControllers} ({_gameControllers.Length}) | Prop {GameControllersProp} ({GameControllersProp.Length})");
            builder.AppendLine($"FindObjectOfType: Field {_drawersScript} | Prop {DrawersScriptProp}");
            builder.AppendLine(
                $"FindObjectsOfType: Field {_drawersScripts} ({_drawersScripts.Length}) | Prop {DrawersScriptsProp} ({DrawersScriptsProp.Length})");
            builder.AppendLine($"LoadResource: Field {_gameObject.name} | Prop {GameObjectProp.name}");

            Debug.Log(builder.ToString());
        }
    }
}