namespace DataAggregator.Db.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
         
        public string Email { get; set; }

        public UserRoleEntity Role { get; set; }

        public string PasswordHash { get; set; }

        public int CountOfRequests { get; set; }

        public DateTime RegistrationDate { get; init; }

        public IEnumerable<ApiTaskEntity> ApiSubscriptions { get; set; }
    }
}
