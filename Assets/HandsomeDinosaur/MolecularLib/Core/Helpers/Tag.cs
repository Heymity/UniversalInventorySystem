using UnityEngine;

namespace MolecularLib.Helpers
{
    [System.Serializable]
    public class Tag
    {
        [SerializeField] private string tag;

        public string TagName { get => tag; set => tag = value; }

        public bool CompareTag(GameObject goToCompareTag) => goToCompareTag.CompareTag(TagName);

        public static implicit operator string(Tag tag) => tag.TagName;
        public static implicit operator Tag(string tag) => new Tag(tag);

        public Tag(string tag)
        {
            TagName = tag;
        }

        public Tag() : this("Untagged") { }
    }

    public static class TagHelper
    {
        public static bool CompareTag(this GameObject go, Tag tagToBeEqual) => go.CompareTag(tagToBeEqual.TagName);
    }
}
