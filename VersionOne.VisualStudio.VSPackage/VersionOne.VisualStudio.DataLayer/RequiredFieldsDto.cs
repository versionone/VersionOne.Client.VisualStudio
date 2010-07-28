namespace VersionOne.VisualStudio.DataLayer {
    internal class RequiredFieldsDto {
        public readonly string Name;
        public readonly string DisplayName;

        public RequiredFieldsDto(string name, string displayName) {
            Name = name;
            DisplayName = displayName;
        }
    }
}