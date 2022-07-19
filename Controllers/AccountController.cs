using System;
using System.Security.Claims;
using System.Text;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegAuthApiDemo.Data;
using RegAuthApiDemo.Domain;
using RegAuthApiDemo.Extention;
using RegAuthApiDemo.Models;
using RegAuthApiDemo.Service;

namespace RegAuthApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Db_Context _db_Context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private  ITokenService _tokenService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, Db_Context db_Context, ITokenService tokenService, IConfiguration configuration, IEmailService emailService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db_Context = db_Context;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
            _mapper = mapper;

        }
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserModel>>GetCurrentUser(){

            var user = await _userManager.FindEmailFromClaimPrinciple(User);

            return Ok(
                 _mapper.Map<UserModel>(user)

                );
           



        }

     

        [HttpGet("emailcheck")]
        
        public async Task<ActionResult<bool>> EmailExist([FromQuery] string email)
        {
          return await _userManager.FindByEmailAsync(email) != null;

        }


        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register(UserModel user)
        {
            var userExist =  await _userManager.FindByEmailAsync(user.Email);
            if (userExist != null)
                return new BadRequestObjectResult("Email already in record");

            User appUser = new User()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Dateofbirth = user.DateofBirth,
                State = user.State,
                Country = user.Country
               



            };

            var result = await _userManager.CreateAsync(appUser, user.Password);

            if (result.Succeeded)
            {
                var userRecond = await _userManager.FindByNameAsync(appUser.UserName);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(userRecond);

                var uriBuild = new UriBuilder(_configuration["ReturnPaths:ConfirmEmail"]);

                var query = HttpUtility.ParseQueryString(uriBuild.Query);
                query["token"] = token;
                query["userid"] = userRecond.Id;
                uriBuild.Query = query.ToString();

                var urlBuild = uriBuild.ToString();

                var senderEmail = _configuration["ReturnPaths:SenderEmail"];

                await _emailService.SendEmailAsync(senderEmail, userRecond.Email, "Confirm you Email", urlBuild);

                return Ok(new
                {
                    token = _tokenService.CreateToken(appUser),
                    Message = "Registeration Successful! Please Confirm your Email Address",
                    Status = "Success"


                });
            }


            if (!result.Succeeded)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    stringBuilder.Append(error.Description);
                    stringBuilder.Append("\r\n");
                }

                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Unable to Register User at this Time" });
            }

               


            //return Ok(new Response { Status = "Success", Message = "Registeration Successful" });


            return BadRequest(new
            {
      
                Message = "Registeration Error! Something bad went wrong",
                Status = "Error"


            });



        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserModel>> Login(UserLogin userLogin)
        {
            var user = await _userManager.FindByNameAsync(userLogin.Email) ??
                await _userManager.FindByEmailAsync(userLogin.Email);

            if (user == null) return Unauthorized(new Response { Status = "Unauthorized", Message = "Failed! No Such Email or UserName in our Record" });

            var result = await _signInManager.PasswordSignInAsync(user, userLogin.Password, true, false);

            if (result.Succeeded)
            {
                var token = _tokenService.CreateToken(user);
                //var tokres = await _userManager.SetAuthenticationTokenAsync(await _userManager.FindByNameAsync(user.UserName), "JWT", "JWT", token);


                return new UserModel
                {
                    Email = user.Email,
                    Token = token,
                    UserName = user.UserName

                };

            }
            if (!result.Succeeded)
                return Unauthorized(new Response { Status = "Unauthorized", Message = "Failed! Invalid Credentials" });
            
                


            return Unauthorized();

        }


        [Authorize]
        [HttpGet("getUsersFromToken")]
        public async Task<ActionResult<UserModel>> GetUserDetailFromtoken()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);

            //user.Address = _mapper.Map<User>(user);

            return Ok(
                   _mapper.Map<UserModel>(user)
                );
            //return new UserModel
            //{
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    UserName = user.UserName,
            //    State = user.State,
            //    Country = user.Country,
            //    PhoneNumber = user.PhoneNumber,
            //    Email = user.Email,
            //    DateofBirth = user.Dateofbirth,

            //};

        }


        [HttpPost("confirmemail")]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmail confirmEmail)
        {
            var user = await _userManager.FindByIdAsync(confirmEmail.UserId);

            var res = await _userManager.ConfirmEmailAsync(user, confirmEmail.Token);

            if (res.Succeeded)
            {
                return Ok();
            }


            return BadRequest();
        }

        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ForgotPassWord(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

            if (user == null)
                return BadRequest(new Response { Message = "Probelm fetching User Record ", Status = "Error" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var uriBuild = new UriBuilder(_configuration["ReturnPaths:ForgetPassword"]);

            var query = HttpUtility.ParseQueryString(uriBuild.Query);
            query["token"] = token;
            query["userid"] = user.Id;
            uriBuild.Query = query.ToString();

            var urlBuild = uriBuild.ToString();

            var senderEmail = _configuration["ReturnPaths:SenderEmail"];

            await _emailService.SendEmailAsync(senderEmail, user.Email, "Click to Confirm Password", urlBuild);


            return Ok(new
            {
                Message = "Check your Email Address for futher Instruction",
                Status = "Success"


            });



        }




        [HttpPost("restPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
           
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                return BadRequest(new Response {
                    Message ="", Status=""});
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest();
            }
            return Ok();
        }
    }
}

