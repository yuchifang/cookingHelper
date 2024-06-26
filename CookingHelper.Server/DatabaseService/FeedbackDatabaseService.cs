using CookingHelper.Data;

public class FeedbackDatabaseService
{
    private readonly UserListDbContext _userListDbContext;

    public FeedbackDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
    }
}
