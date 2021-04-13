delete SupportChats where UserID in (select UserID from Users where MainMobile is null)
delete Users where MainMobile is null
