# LondonStockApi

## System Design for MVP

In the simple version of the API that I've implemented, there are two controller classes: TradeController and StockValueController.

The TradeController class contains one endpoint that is used for receiving trade notifications and writing the trades to the database.  Each trade is validated before being written to the database.

The StockValueController class contains three endpoints that are used for retrieving the current value of all stocks, a single stock, and a named list of stocks.  The value of a stock is considered to be the average price of the stock across all transactions (in this calculation I chose to ignore the number of shares per transaction).

## Enhancements

There are a couple of problems that mean that this system is not scalable:

- The Notify endpoint in the TradeController class is a bottleneck, and it's likely that its performance will degrade with high traffic.
- As the number of trades in the database increases, the queries made by the StockValueService class will return more and more data, and so the StockValueController endpoints will become slower and slower.

The problem of the Notify endpoint being a bottleneck can be overcome by making the following changes to the design:

- Deploy multiple instances of the API behind a load balancer.
- Shard the database, i.e. split the data across multiple databases.  The sharding should be done in such a way that all trades for a given ticker are stored in the same shard.

The problem of the queries in the StockValueService class becoming slower as the database grows can be overcome as follows:

- Add a StockValues database table that contains Ticker and Price columns (this table can also be split across multiple shards).
- Each time a new trade is inserted into the Trades table, an updated value should be calculated for the trade's stock, and the appropriate row should be updated in the StockValues table.
- When calculating the updated stock value, we can avoid having to query the Trades table every time by maintaining an in-memory cache of recent trades (e.g. using Redis), and calculating the stock value based on the cached trades.
- The queries in the StockValueService class can then be repointed to the StockValues table.  This should not have the same performance problem as querying the Trades table, because the StockValues table will not grow at the same rate as the Trades table.

To guarantee system availablity it would also be a good idea to use database replication to ensure that we have more than one copy of the data available.  We can then temporarily divert queries to the replica if there is a problem with the master database.
