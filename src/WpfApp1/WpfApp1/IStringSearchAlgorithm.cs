using System.Collections.Generic;

public interface IStringSearchAlgorithm
{
    IEnumerable<(int Position, int HammingDistance, double ClosenessPercentage)> Search(string text, string pattern);
}
