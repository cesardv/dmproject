This data is from https://wiki.csc.calpoly.edu/datasets/wiki/apriori

    The XXXX-out1.csv file format is: receipt# followed by item #'s that are on that receipt. (sparse vector representation)
    The XXXX-out2.csv file format is: receipt# followed by 0's and 1's indicating if an item was on a given receipt. (full binary vector representation).
    The XXXXi.csv file format is: receipt# followed by item number and quantity ( CSV version of the Items tables ) 

The below are equivalent on a size 10 dataset (ours is size 50)

XXXX-out1.csv: 3,0,2,4,6

XXXX-out2.csv: 3,1,0,1,0,1,0,1,0,0,0

XXXXi.csv :

    3, 0, 1
    3, 2, 1
    3, 4, 1
    3, 6, 1

xxxx = tell the number of transaction in said dataset