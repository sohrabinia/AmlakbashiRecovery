DECLARE @duplicatedMobile TABLE
(
   MainMobile nvarchar(100)
);
insert into @duplicatedMobile
 SELECT MainMobile FROM Users WHERE MainMobile IN
  (SELECT MainMobile FROM Users GROUP BY MainMobile HAVING COUNT(*) > 1)
  group by MainMobile

   --select MainMobile, (select top 1(CreateDate) from Reserves where UserID = Users.UserID) from Users where MainMobile in (select MainMobile from @duplicatedMobile)
   --and State = 1 and UserID in (select UserID from Reserves) order by MainMobile

  delete from Users where MainMobile in (select MainMobile from @duplicatedMobile) and State != 1 and UserID not in (select UserID from Reserves) and UserID not in (select UserID from Advertises) and UserID not in (select UserID from ActionLogs)

  delete from @duplicatedMobile

  insert into @duplicatedMobile
	SELECT MainMobile FROM Users WHERE MainMobile IN
  (SELECT MainMobile FROM Users GROUP BY MainMobile HAVING COUNT(*) > 1)
  group by MainMobile
  ;
 --select UserID,MainMobile,CreateDate from Users where MainMobile in (select MainMobile from @duplicatedMobile)
 --order by MainMobile, State, CreateDate

 DECLARE @usersToMerge TABLE
(
   UserID bigint,
   MainMobile nvarchar(100),
   TargetUserID bigint
);

 WITH cte AS (
    SELECT 
        UserID,MainMobile,CreateDate,
        ROW_NUMBER() OVER (
            PARTITION BY 
                MainMobile
            ORDER BY
                MainMobile, 
                State,
				email desc,
                CreateDate
        ) row_num
     FROM 
        Users
		where MainMobile in (select MainMobile from @duplicatedMobile)
)
insert into @usersToMerge
select UserID, MainMobile,(select UserID from cte where ccc.MainMobile = MainMobile and row_num = 1)
from cte ccc where row_num > 1

--select * from @usersToMerge

update Reserves set UserID = (select TargetUserID from @usersToMerge where UserID = Reserves.UserID)
where UserID in (select UserID from @usersToMerge)

update Reserves set HostUserID = (select TargetUserID from @usersToMerge where UserID = Reserves.HostUserID)
where HostUserID in (select UserID from @usersToMerge)

update Advertises set UserID = (select TargetUserID from @usersToMerge where UserID = Advertises.UserID)
where UserID in (select UserID from @usersToMerge)

update ActionLogs set UserID = (select TargetUserID from @usersToMerge where UserID = ActionLogs.UserID)
where UserID in (select UserID from @usersToMerge)

update BankCards set UserID = (select TargetUserID from @usersToMerge where UserID = BankCards.UserID)
where UserID in (select UserID from @usersToMerge)

update BlogPosts set UserID = (select TargetUserID from @usersToMerge where UserID = BlogPosts.UserID)
where UserID in (select UserID from @usersToMerge)

update BlogPosts set LastModifyUserID = (select TargetUserID from @usersToMerge where LastModifyUserID = BlogPosts.UserID)
where LastModifyUserID in (select UserID from @usersToMerge)

update Carts set UserID = (select TargetUserID from @usersToMerge where UserID = Carts.UserID)
where UserID in (select UserID from @usersToMerge)

update Payments set UserID = (select TargetUserID from @usersToMerge where UserID = Payments.UserID)
where UserID in (select UserID from @usersToMerge)

update CreditTransactions set UserID = (select TargetUserID from @usersToMerge where UserID = CreditTransactions.UserID)
where UserID in (select UserID from @usersToMerge)

update PrizeCreditTransactions set UserID = (select TargetUserID from @usersToMerge where UserID = PrizeCreditTransactions.UserID)
where UserID in (select UserID from @usersToMerge)

update Chats set UserID = (select TargetUserID from @usersToMerge where UserID = Chats.UserID)
where UserID in (select UserID from @usersToMerge)

update Comments set SenderUserID = (select TargetUserID from @usersToMerge where UserID = Comments.SenderUserID)
where SenderUserID in (select UserID from @usersToMerge)

update Comments set RecieverUserID = (select TargetUserID from @usersToMerge where UserID = Comments.RecieverUserID)
where RecieverUserID in (select UserID from @usersToMerge)

update DiscountCoupons set UserID = (select TargetUserID from @usersToMerge where UserID = DiscountCoupons.UserID)
where UserID in (select UserID from @usersToMerge)

update ExtrinsicReserves set NotifierUserID = (select TargetUserID from @usersToMerge where UserID = ExtrinsicReserves.NotifierUserID)
where NotifierUserID in (select UserID from @usersToMerge)

update ExtrinsicReserves set HostUserID = (select TargetUserID from @usersToMerge where UserID = ExtrinsicReserves.HostUserID)
where HostUserID in (select UserID from @usersToMerge)

update Files set UserID = (select TargetUserID from @usersToMerge where UserID = Files.UserID)
where UserID in (select UserID from @usersToMerge)

update Posts set UserID = (select TargetUserID from @usersToMerge where UserID = Posts.UserID)
where UserID in (select UserID from @usersToMerge)

update ReportItems set UserID = (select TargetUserID from @usersToMerge where UserID = ReportItems.UserID)
where UserID in (select UserID from @usersToMerge)

update ReportItems set OperatorID = (select TargetUserID from @usersToMerge where OperatorID = ReportItems.UserID)
where OperatorID in (select UserID from @usersToMerge)

update ReservePayments set UserID = (select TargetUserID from @usersToMerge where UserID = ReservePayments.UserID)
where UserID in (select UserID from @usersToMerge)

update ReservePayments set OperatorID = (select TargetUserID from @usersToMerge where OperatorID = ReservePayments.UserID)
where OperatorID in (select UserID from @usersToMerge)

update ReserveSupports set SupporterID = (select TargetUserID from @usersToMerge where UserID = ReserveSupports.SupporterID)
where SupporterID in (select UserID from @usersToMerge)

update ReserveSupports set GuestID = (select TargetUserID from @usersToMerge where UserID = ReserveSupports.GuestID)
where GuestID in (select UserID from @usersToMerge)

update SupportChatMessages set UserID = (select TargetUserID from @usersToMerge where UserID = SupportChatMessages.UserID)
where UserID in (select UserID from @usersToMerge)

update SupportChats set UserID = (select TargetUserID from @usersToMerge where UserID = SupportChats.UserID)
where UserID in (select UserID from @usersToMerge)

update SupportChats set SupporterID = (select TargetUserID from @usersToMerge where UserID = SupportChats.SupporterID)
where SupporterID in (select UserID from @usersToMerge)

update UserFavorites set User_Id = (select TargetUserID from @usersToMerge where UserID = UserFavorites.User_Id)
where User_Id in (select UserID from @usersToMerge)

delete from Users where UserID in (select UserID from @usersToMerge)




DECLARE @user_id int

DECLARE user_cursor CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT UserID 
FROM Users where UserID in (select TargetUserID from @usersToMerge)
OPEN user_cursor
FETCH NEXT FROM user_cursor INTO @user_id
WHILE @@FETCH_STATUS = 0
BEGIN 
	Declare @initial_credit bigint
	set @initial_credit = (select top 1(RemainedPrice - Price) from CreditTransactions where UserID = @user_id)
	declare @user_credit_id bigint
	declare user_credit_cursor cursor
	local static read_only forward_only
	for select distinct CreditTransactionID
	from CreditTransactions where UserID = @user_id
	open user_credit_cursor
	fetch next from user_credit_cursor into @user_credit_id
	while @@FETCH_STATUS = 0
	begin
	update CreditTransactions set RemainedPrice = @initial_credit + Price where CreditTransactionID = @user_credit_id
	set @initial_credit = (select top 1(RemainedPrice) from CreditTransactions where CreditTransactionID = @user_credit_id)
	fetch next from user_credit_cursor into @user_credit_id
	end
	close user_credit_cursor
	deallocate user_credit_cursor
	if @initial_credit is not null update Users set Credit = @initial_credit where UserID = @user_id

	declare @initial_prize_credit bigint
	set @initial_prize_credit = 0
	declare @user_prize_credit_id bigint
	declare user_prize_credit_cursor cursor
	local static read_only forward_only
	for select distinct ID
	from PrizeCreditTransactions where UserID = @user_id
	open user_prize_credit_cursor
	fetch next from user_prize_credit_cursor into @user_prize_credit_id
	while @@FETCH_STATUS = 0
	begin
	update PrizeCreditTransactions set RemainedPrice = @initial_prize_credit + Price where ID = @user_prize_credit_id
	set @initial_prize_credit = (select top 1(RemainedPrice) from PrizeCreditTransactions where ID = @user_prize_credit_id)
	fetch next from user_credit_cursor into @user_credit_id
	end
	close user_prize_credit_cursor
	deallocate user_prize_credit_cursor
	if @initial_prize_credit is not null update Users set PrizeCredit = @initial_prize_credit where UserID = @user_id

    PRINT @user_id
    FETCH NEXT FROM user_cursor INTO @user_id
END
CLOSE user_cursor
DEALLOCATE user_cursor
 
