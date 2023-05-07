using System.IO;

namespace MolecularLib.PolymorphismSupport
{
    public interface IPolymorphicSerializationOverride
    {
        void Deserialize(string reader);

        void Serialize(StringWriter writer);
    }
}