# C# Recommendation Engine

This project and [corresponding article](https://www.codeproject.com/Articles/1232150/Building-an-Article-Recommendation-Engine) won first place in Code Project's 2018 Machine Learning and Artificial Intelligence Competition.
It demonstrates user-based and item-based collaborative filtering with matrix factorization using
the user behavior data provided by Code Project for the challenge.

### Usage

```C#
IRater rate = new LinearRater(-4, 2, 3, 1);
IComparer compare = new CorrelationUserComparer();
IRecommender recommender = new UserCollaborativeFilterRecommender(compare, rate, 50);

UserBehaviorDatabaseParser parser = new UserBehaviorDatabaseParser();
UserBehaviorDatabase db = parser.LoadUserBehaviorDatabase("UserBehavior.txt");
ISplitter split = new DaySplitter(db, 5);

recommender.Train(split.TrainingDB);

ScoreResults scores = recommender.Score(split.TestingDB, rate);
TestResults results = recommender.Test(split.TestingDB, 30);

List<Suggestion> suggestions = recommender.GetSuggestions(someUserId, numberOfRecommendations);
```
