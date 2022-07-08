namespace DataAggregator.Dal.Contract.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
         
        public string Email { get; set; }

        public UserRoleDto Role { get; set; }

        public string PasswordHash { get; set; }

        public int CountOfRequests { get; set; }

        public DateTime RegistrationDate { get; init; }

        public IEnumerable<ApiTaskDto> ApiSubscriptions { get; set; }
    }
}
