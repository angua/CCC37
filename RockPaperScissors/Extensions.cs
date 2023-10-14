namespace RockPaperScissors
{
    internal static class Extensions
    {
        public static char GetOutCome(this string fight) => GetOutCome(fight[0], fight[1]);

        public static char GetOutCome(char first, char second)
        {
            if (first == second)
            {
                return first;
            }

            switch (first)
            {
                case 'R':
                    return second switch
                    {
                        'P' => 'P', // paper beats rock
                        'S' => 'R', // rock beats scissors
                        _ => first
                    };

                case 'P':
                    return second switch
                    {
                        'R' => 'P', // paper beats rock
                        'S' => 'S', // scissors beats paper
                        _ => first
                    };

                case 'S':
                    return second switch
                    {
                        'P' => 'S', // scissors beats paper
                        'R' => 'R', // rock beats scissors
                        _ => first
                    };
            }

            throw new ArgumentOutOfRangeException("Invalid fighting style");
        }
    }
}
