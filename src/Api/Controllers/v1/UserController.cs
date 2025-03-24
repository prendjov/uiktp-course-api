using Application.Queries;
using Application.Services;
using AutoMapper;
using DTO.Enums.User;
using DTO.Medias;
using DTO.Response;
using DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class UserController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(IMapper mapper,
                          IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<UserResponse> Create([FromBody] UserCreateRequest request)
    {
        return await _userService.CreateUser(request);
    }

    [HttpPut()]
    public async Task<UserResponse> Update([FromBody] UserUpdateRequest request)
    {
        return await _userService.UpdateUser(request);
    }

    [HttpGet("{id:int}")]
    public async Task<UserInfoResponse> Get([FromQuery] int id)
    {
        var response = await _userService.GetById(id);

        return response;
    }

    [HttpGet("me")]
    public async Task<MeResponse> GetUserInfo()
    {
        var response = await _userService.GetMe();

        return response;
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
    {
        await _userService.ChangePassword(request);
        return Ok();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        await _userService.ForgotPassword(email);
        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordRequest request)
    {
        try
        {
            await _userService.ResetPassword(request);
            return Ok();
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("profile-picture")]
    public async Task<MediaItemResponse> UpdateProfilePicture([FromForm] UserProfilePictureUpdateRequest request)
    {
        var response = await _userService.UpdateProfilePicture(request.Picture);
        return response;
    }

    [HttpPut("activate")]
    public async Task<IActionResult> Activate()
    {
        await _userService.ChangeStatus(UserStatus.Active);
        return Ok();
    }

    [HttpPut("deactivate")]
    public async Task<IActionResult> Deactivate()
    {
        await _userService.ChangeStatus(UserStatus.Deactivated);
        return Ok();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}/suspend")]
    public async Task<IActionResult> Suspend([FromRoute] int id, [FromBody] UserSuspendRequest request)
    {
        await _userService.SuspendUser(id, request.SuspensionReason);
        return Ok();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}/remove-suspension")]
    public async Task<IActionResult> Unsuspend([FromRoute] int id)
    {
        await _userService.UnsuspendUser(id);
        return Ok();
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("status")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetStatuses()
    {
        return await Mediator.Send(new GetEnumValuesQuery(typeof(UserStatus)));
    }
}
