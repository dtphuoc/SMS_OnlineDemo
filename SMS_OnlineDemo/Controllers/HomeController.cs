using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SMS_OnlineDemo.Models;
using System.Threading.Tasks;

namespace SMS_OnlineDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        DemoSMS_OnlienEntities3 db = new DemoSMS_OnlienEntities3();

        [HttpGet]
        public ActionResult Chat()
        {
            var userMessages = from usersChatBoard in db.ChatBoards
                               orderby usersChatBoard.DateTimeOfMessage descending
                               select usersChatBoard;

            ViewBag.ChatBoardList = userMessages.ToList();


            //var onlineUsers = from user in db.Users
            //                  where user.Status == "Logged In"
            //                  select user;

            //ViewBag.UsersOnlineList = onlineUsers.ToList();

            return View();
        }



        [HttpPost]
        public async Task<ActionResult> Chat(SMS_OnlineDemo.Models.ChatBoard chatModel)
        {
            Models.User user = new Models.User();

            Models.ChatBoard chatBoard = new ChatBoard();
            //chatBoard.FromUser = Session["FromUser"].ToString();
            chatBoard.Message = chatModel.Message;
            chatBoard.DateTimeOfMessage = chatModel.DateTimeOfMessage;

            db.ChatBoards.Add(chatBoard);
            await db.SaveChangesAsync();

            return RedirectToAction("Chat", "Home");
        }
    }
}