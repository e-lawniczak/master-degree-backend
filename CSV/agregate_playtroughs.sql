
select  u.UserId, UserName, u.IsControlGroup, HighScore,
SUM(p.TotalTime) as TotalTime_SUM, 
AVG(p.TotalTime) as TotalTime_AVG, 
AVG(PercentageProgress) as Progress_AVG, 
MAX(PercentageProgress) as Progress_MAX, 
AVG(TotalEnemyProxTime) as EnemyProximity_AVG
,SUM(len(DefeatedEnemiesIds)) as defeted_str
--,SUM(len(DefeatedEnemiesIds) + 1)/4 as defeted_int
,SUM(len(CollectedCoinsIds)) as collected_str
--,SUM(len(CollectedCoinsIds)+1)/4 as collected_int
--(SELECT DefeatedEnemiesIds, 
--	CASE len(DefeatedEnemiesIds) WHEN 0 THEN 0
--	ELSE len(DefeatedEnemiesIds) - len(replace(DefeatedEnemiesIds, ',', '')) END
--	 FROM [ErykMgr].[dbo].[Users] su join Playtroughs sp on u.UserId = p.UserId
--	 WHERE su.UserId = u.UserId
--) as Defeated_enemies

  FROM [ErykMgr].[dbo].[Users] u join Playtroughs p on u.UserId = p.UserId
where IsControlGroup =1
   Group by u.UserId, UserName, IsControlGroup
   ,HighScore
  -- DefeatedEnemiesIds,
--CollectedCoinsIds
order by TotalTime_Sum desc 
