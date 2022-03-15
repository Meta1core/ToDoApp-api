namespace ToDoApp.Models.DTO
{
    public class RegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsValid()
        {
            if (Password.Equals(ConfirmPassword) && !string.IsNullOrEmpty(Email))
            {
                return true;
            }
            return false;
        }
    }
}