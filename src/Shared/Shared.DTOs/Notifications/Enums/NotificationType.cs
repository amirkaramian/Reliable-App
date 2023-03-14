using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Notifications.Enums;

public enum NotificationType
{
    NEW_USER_REGISTERED,
    TICKET,
    TICKET_CREATED,
    TICKET_UPDATED,
    ORDER,
    ORDER_CREATED,
    ORDER_UPDATED,
    TICKET_NEW_COMMENTS,
    TICKET_NEW_REPLY,
    CATEGORY,
    CATEGORY_GENERATOR,
    BILLS,
    BILL_CREATED,
    ADDED_TO_ADMIN_GROUP,
    ADDED_TO_DEPARTMENT_GROUP,
    ARTICLE_FEEDBACK_ADDED,
    ARTICLE_FEEDBACK_COMMENT_ADDED,
    ARTICLE_FEEDBACK_COMMENT_REPLY_ADDED,
    PRODUCT_STATUS_UPDATED,
    NEW_SUBUSER_CREATED
}
