DECLARE @duplicatedMobile TABLE
(
   MainMobile nvarchar(100)
);
DECLARE @deletedUsers TABLE
(
	UserID int
);
insert into @deletedUsers
select UserID from SupportChats where UserID in (select UserID from Users where MainMobile is null)
insert into @deletedUsers
select UserID from Users where MainMobile is null
insert into @duplicatedMobile
 SELECT MainMobile FROM Users WHERE MainMobile IN
  (SELECT MainMobile FROM Users GROUP BY MainMobile HAVING COUNT(*) > 1)
  group by MainMobile

   --select MainMobile, (select top 1(CreateDate) from Reserves where UserID = Users.UserID) from Users where MainMobile in (select MainMobile from @duplicatedMobile)
   --and State = 1 and UserID in (select UserID from Reserves) order by MainMobile
   insert into @deletedUsers
  select UserID from Users where MainMobile in (select MainMobile from @duplicatedMobile) and State != 1 and UserID not in (select UserID from Reserves) and UserID not in (select UserID from Advertises) and UserID not in (select UserID from ActionLogs)

  delete from @duplicatedMobile

  insert into @duplicatedMobile
	SELECT MainMobile FROM Users WHERE MainMobile IN
  (SELECT MainMobile FROM Users GROUP BY MainMobile HAVING COUNT(*) > 1)
  group by MainMobile
  ;

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
insert into @deletedUsers
select UserID
from cte where row_num > 1

SELECT * FROM Users where UserID in (select UserID from @deletedUsers) FOR XML PATH

