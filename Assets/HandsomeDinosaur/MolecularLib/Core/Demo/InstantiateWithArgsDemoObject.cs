using UnityEngine;
using UnityEngine.UI;

namespace MolecularLib.Demo
{
    // For any class that you want to have a Initialize method, you will inherit from this interface, IArgsInstantiable<> with up to 10 generic arguments.
    // The type of the generic method will match the types of the parameters received in the Initialize method.
    // You can have more than one of these interfaces and methods per class.
    // To use it, call Molecular.Instantiate(originalObj, arg0, arg1, ...), where the originalObj is this, or any other with the interface, class.
    // There are also other overloads of the method like Molecular.Instantiate(originalObject, position, rotation, arg0, arg1, ...) and others.
    // The way this works under the hood does NOT use reflection, so it is very fast.
    // Beware that the Initialize method will be called after the Awake method but before Start, since the Initialize method is called right after the object is instantiated.
    public class InstantiateWithArgsDemoObject : MonoBehaviour, IArgsInstantiable<string>
    {
        public string myString;
        public Text stringText;   
        
        public void Initialize(string str)
        {
            myString = str;
            stringText.text = myString;
        }
    }
}
