# C# Recommendation Engine

[Read my Code Project article!](https://www.codeproject.com/Articles/1232150/Building-an-Article-Recommendation-Engine)

This project demonstrates user-based and item-based collaborative filtering with matrix factorization using
the user behavior data provided by Code Project for the 2018 AI Challenge.


```C#
IRater rate = new LinearRater(-4, 2, 3, 1);
IComparer compare = new CorrelationUserComparer();
IRecommender recommender = new UserCollaborativeFilterRecommender(compare, rate, 50);

UserBehaviorDatabaseParser parser = new UserBehaviorDatabaseParser();
UserBehaviorDatabase db = parser.LoadUserBehaviorDatabase("UserBehavior.txt");
ISplitter split = new DaySplitter(db, 5);

recommender.Train(split.TrainingDB);


```