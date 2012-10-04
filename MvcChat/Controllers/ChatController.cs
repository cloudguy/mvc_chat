using System;
using System.Linq;
using System.Web.Mvc;
using MvcChat.Domain;
using MvcChat.Implementation;
using MvcChat.Interfaces;
using MvcChat.Models;
using MvcChat.Util;

namespace MvcChat.Controllers
{
    public class ChatController : Controller
    {
        private const int LatestVisibleMessages = 10;

        //ioc should be used for instantating services by creating custom controller factory
        //but for this example this would work
        private readonly IMessageRepository _messageRepo = new InProcMessageRepository();

        public ActionResult Index()
        {
            return View(new NewMessageModel());
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult LoadInitialData()
        {
            var model = new MessageListModel
            {
                Messages = _messageRepo.GetLatest(LatestVisibleMessages).Select(m => new ChatMessageDto(m))
            };
            return Json(model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckNewMessages(int? lastCheckedId)
        {
            var model = new MessageListModel
            {
                Messages = _messageRepo.GetLatestAfterId(lastCheckedId ?? 0).Select(m => new ChatMessageDto(m))
            };
            return Json(model);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AjaxOnly]
        public ActionResult Send(NewMessageModel chatMessage)
        {
            if (ModelState.IsValid)
            {
                _messageRepo.Insert(new ChatMessage
                    {
                        SendTime = DateTimeOffset.UtcNow,
                        Text = chatMessage.Message,
                        UserName = chatMessage.UserName
                    });
                var model = new MessageListModel
                {
                    Messages = _messageRepo.GetLatestAfterId(chatMessage.LastCheckedId ?? 0).Select(m => new ChatMessageDto(m))
                };
                return Json(model);
            }
            GenericErrorModel errorModel = new GenericErrorModel().SetModelErrors(ViewData);
            return Json(errorModel);
        }
    }
}
