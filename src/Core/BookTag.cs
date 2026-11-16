namespace DeviceOfHermes;

/// <summary>A book tag</summary>
public static class BookTag
{
    /// <summary>Set book tags</summary>
    public static void SetTags(LorId target, params List<string> tags)
    {
        dict[target] = tags;
    }

    extension(BookModel book)
    {
        /// <summary>Returns all tags</summary>
        public List<string> GetTags()
        {
            var id = book.ClassInfo?.id;

            if (id is not null && dict.TryGetValue(id, out var tags))
            {
                return new(tags);
            }

            return new();
        }

        /// <summary>Returns true if tag exists</summary>
        public bool ExistsTag(string tag)
        {
            var id = book.ClassInfo?.id;

            return id is not null && dict.TryGetValue(id, out var tags) && tags.Contains(tag);
        }
    }

    static Dictionary<LorId, List<string>> dict = new();
}
