﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MrHuo.OAuth.Wechat;
using MrHuo.OAuth.Github;

namespace MrHuo.OAuth.NetCoreApp.Controllers
{
    public class OAuthController : Controller
    {
        private readonly GithubOAuth githubOauth = null;
        private readonly WechatOAuth wechatOAuth = null;
        public OAuthController(GithubOAuth githubOauth, WechatOAuth wechatOAuth)
        {
            this.githubOauth = githubOauth;
            this.wechatOAuth = wechatOAuth;
        }

        [HttpGet("oauth/{type}")]
        public void Index(string type)
        {
            switch (type.ToLower())
            {
                case "github":
                    {
                        githubOauth.Authorize();
                        break;
                    }
                case "wechat-qrcode":
                    {
                        wechatOAuth.SetWechatOAuthType(WechatOAuthType.Qrcode);
                        wechatOAuth.Authorize();
                        break;
                    }
                case "wechat-client":
                    {
                        wechatOAuth.SetWechatOAuthType(WechatOAuthType.Client);
                        wechatOAuth.Authorize();
                        break;
                    }
                default:
                    HttpContext.Response.StatusCode = 404;
                    break;
            }
        }

        [HttpGet("oauth/{type}callback")]
        public IActionResult LoginCallback(string type)
        {
            switch (type.ToLower())
            {
                case "github":
                    {
                        var accessToken = githubOauth.GetAccessToken(Request.Query["code"], Request.Query["state"]);
                        var userInfo = githubOauth.GetUserInfo(accessToken);
                        return Json(new
                        {
                            accessToken,
                            userInfo
                        });
                    }
                case "wechat":
                    {
                        var accessToken = wechatOAuth.GetAccessToken(Request.Query["code"], Request.Query["state"]);
                        var userInfo = wechatOAuth.GetUserInfo(accessToken);
                        return Json(new
                        {
                            accessToken,
                            userInfo
                        });
                    }
            }
            return Content($"没有实现【{type}】登录！");
        }
    }
}
