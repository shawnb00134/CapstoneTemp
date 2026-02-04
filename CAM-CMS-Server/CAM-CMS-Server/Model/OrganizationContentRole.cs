namespace CAMCMSServer.Model
{
    public class OrganizationContentRole
    {
        #region Data member

        public int OrganizationContentRoleId { get; set; }

        public int OrganizationId { get; set; }

        public int? ContentRoleId { get; set; }

        public List<int> ArchetypeIds { get; set; }

        public string? DisplayName { get; set; }

        public string? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public string? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        #endregion

        #region Methods
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OrganizationContentRole)obj;

            return this.OrganizationContentRoleId == other.OrganizationContentRoleId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
