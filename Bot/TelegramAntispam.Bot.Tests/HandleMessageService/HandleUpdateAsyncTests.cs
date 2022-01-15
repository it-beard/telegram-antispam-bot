using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramAntispam.Bot.Interfaces;
using Xunit;

namespace TelegramAntispam.Bot.Tests.HandleMessageService;

public class HandleUpdateAsyncTests
{
    readonly Mock<IDeleteMessageService> deleteMessageService;
    readonly Mock<ITelegramBotClient> botClient;
    readonly CancellationToken cancellationToken;

    public HandleUpdateAsyncTests()
    {
        deleteMessageService = new Mock<IDeleteMessageService>();
        botClient = new Mock<ITelegramBotClient>();
        cancellationToken = new CancellationToken();
    }
    
    [Fact]
    public void IfEmptyMessage_DeleteNotCalled()
    {
        //Arrange
        var updateType = It.IsAny<UpdateType>();
        var update = new Update { Message = null };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, updateType, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    public void IUpdateTypeIsMessageAndFromChannel_DeleteNotCalled()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Telegram"
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.Message, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    public void IUpdateTypeIsEditedMessageAndFromChannel_DeleteNotCalled()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "Telegram"
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.EditedMessage, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
                m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public void IUpdateTypeIsMessageAndNotFromChannelAndUserInWhiteListAndContainsUrl_DeleteNotCalled()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = Settings.WhiteList[0]
                },
                Text = $"Test message with word {Settings.NoCommentWord}",
                Entities = new []
                {
                    new MessageEntity
                    {
                        Type = MessageEntityType.Url
                    }
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.Message, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    public void IUpdateTypeIsEditedMessageAndNotFromChannelAndUserInWhiteListAndContainsUrl_DeleteNotCalled()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = Settings.WhiteList[0]
                },
                Text = $"Test message with word {Settings.NoCommentWord}",
                Entities = new []
                {
                    new MessageEntity
                    {
                        Type = MessageEntityType.Url
                    }
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.EditedMessage, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
                m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    public void IUpdateTypeIsMessageAndFromChannelAndContainsNoCommentWord_DeleteCalledOnce()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "Telegram"
                },
                Text = $"Test message with word {Settings.NoCommentWord}"
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.Message, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public void IUpdateTypeIsMessageAndNotFromChannelAndUserNotInWhiteListAndContainsUrl_DeleteCalledOnce()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName"
                },
                Text = $"Test message with word {Settings.NoCommentWord}",
                Entities = new []
                {
                    new MessageEntity
                    {
                        Type = MessageEntityType.Url
                    }
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.Message, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
        
    [Fact]
    public void IUpdateTypeIsEditedMessageAndFromChannelAndContainsNoCommentWord_DeleteCalledOnce()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "Telegram"
                },
                Text = $"Test message with word {Settings.NoCommentWord}"
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.EditedMessage, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public void IUpdateTypeIsEditedMessageAndNotFromChannelAndUserNotInWhiteListAndContainsUrl_DeleteCalledOnce()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName"
                },
                Text = $"Test message with word {Settings.NoCommentWord}",
                Entities = new []
                {
                    new MessageEntity
                    {
                        Type = MessageEntityType.Url
                    }
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.EditedMessage, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
            m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public void IUpdateTypeIsEditedMessageAndNotFromChannelAndUserNotInWhiteListAndContainsUrlInCaption_DeleteCalledOnce()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName"
                },
                Text = $"Test message with word {Settings.NoCommentWord}",
                CaptionEntities = new []
                {
                    new MessageEntity
                    {
                        Type = MessageEntityType.Url
                    }
                }
            }
        };
        var service = new Services.HandleMessageService(deleteMessageService.Object);
        
        //Act
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.EditedMessage, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
                m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}