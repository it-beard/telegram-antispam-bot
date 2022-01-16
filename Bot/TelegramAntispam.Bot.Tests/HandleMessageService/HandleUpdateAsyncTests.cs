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
    public void IfEmptyMessage_Skip()
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
    public void IfEditedMessageFromOwnChannel_Skip()
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
    public void IfMessageFromChannel_Skip()
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
    public void IfEditedMessageWithLinkFromUserInWhiteList_Skip()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = Settings.WhiteList[0],
                    IsBot = false
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
    public void IfMessageWithLinkFromUserInWhiteList_Skip()
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
    public void IfEditedMessageWithNoCommentWordFromOwnChannel_Delete()
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
    public void IsMessageWithNoCommentWordFromOwnChannel_Delete()
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
    public void IfEditedMessageWithLinkFromUserNotInWhiteList_Delete()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName",
                    IsBot = false
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
    public void IfMessageWithLinkFromUserNotInWhiteList_Delete()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName",
                    IsBot = false
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
    public void IfEditedMessageWithLinkInCaptionFromUserNotInWhiteList_Delete()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName",
                    IsBot = false
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
    
    [Fact]
    public void IfMessageWithLinkInCaptionFromUserNotInWhiteList_Delete()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                From = new User
                {
                    FirstName = "testUserName",
                    Username = "notInWhiteListUserName",
                    IsBot = false
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
        service.HandleUpdateAsync(botClient.Object, update, UpdateType.Message, cancellationToken);
        
        //Assert
        deleteMessageService.Verify(m => 
                m.DeleteMessageAsync(It.IsAny<ITelegramBotClient>(), It.IsAny<Message>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public void IfEditedMessageFromChannelNotInWhiteList_Delete()
    {
        //Arrange
        var update = new Update
        {
            EditedMessage = new Message
            {
                Text = "example text",
                From = new User
                {
                    IsBot = true
                },
                SenderChat = new Chat()
                {
                    Username = "channelNotFromChannelsWhiteList"
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
    public void IfMessageFromChannelNotInWhiteList_Delete()
    {
        //Arrange
        var update = new Update
        {
            Message = new Message
            {
                Text = "example text",
                From = new User
                {
                    IsBot = true
                },
                SenderChat = new Chat()
                {
                    Username = "channelNotFromChannelsWhiteList"
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
}