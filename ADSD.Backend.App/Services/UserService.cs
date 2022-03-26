using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Json;

namespace ADSD.Backend.App.Services;

public class UserService
{
    private readonly AppDbClient _appDbClient;

    public UserService(AppDbClient appDbClient)
    {
        _appDbClient = appDbClient;
    }

    public IEnumerable<UserBaseInfo> GetUsers()
    {
        var users = _appDbClient.GetUsers()
            .ToList();

        foreach (var user in users)
        {
            user.Roles = _appDbClient.GetRolesByUserId(user.Id)
                .Select(x => x.ToString())
                .ToList();
            yield return user;
        }
    }

    public UserFullInfo GetUser(int id)
    {
        var user = _appDbClient.GetUserById(id);
        user.Roles = _appDbClient.GetRolesByUserId(user.Id)
            .Select(x => x.ToString())
            .ToList();
        return user;
    }

    public int CreateUser(UserFullInfo userFullInfo)
    {
        return _appDbClient.CreateUser(userFullInfo);
    }

    public void UpdateUser(int id, UserFullInfo userFullInfo)
    {
        _appDbClient.UpdateUser(id, userFullInfo);
    }
}