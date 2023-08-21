using System.Collections.Generic;
using UnityEngine;

namespace MolecularLib.Helpers
{
    [System.Serializable]
    public class Tag
    {
        public static readonly Tag Untagged = new Tag("Untagged");
        public static readonly Tag Respawn = new Tag("Respawn");
        public static readonly Tag Finish = new Tag("Finish");
        public static readonly Tag EditorOnly = new Tag("EditorOnly");
        public static readonly Tag MainCamera = new Tag("MainCamera");
        public static readonly Tag Player = new Tag("Player");
        public static readonly Tag GameController = new Tag("GameController");

        [SerializeField] private string tag;

        public string TagName { get => tag; set => tag = value; }

        public bool CompareTag(GameObject goToCompareTag) => goToCompareTag.CompareTag(TagName);

        public static implicit operator string(Tag tag) => tag.TagName;
        public static implicit operator Tag(string tag) => new Tag(tag);

        public static bool operator ==(Tag t1, Tag t2)
        {
            if (t1 is null && t2 is null) return true;
            if (t1 is null || t2 is null) return false;
            return t1.TagName == t2.TagName;
        }

        public static bool operator !=(Tag t1, Tag t2)
        {
            return !(t1 == t2);
        }
        
        protected bool Equals(Tag other) => tag == other.tag;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Tag)obj);
        }

        public override int GetHashCode() => tag != null ? tag.GetHashCode() : 0;


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
