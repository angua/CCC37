namespace RockPaperScissors
{
    public static class Extensions
    {
        public static char ToChar(this Fighter fighter)
        {
            return fighter switch
            {
                Fighter.Rock => 'R',
                Fighter.Paper => 'P',
                Fighter.Scissors => 'S',
                Fighter.Lizard => 'L',
                Fighter.Spock => 'Y',
                Fighter.Available => 'X',
                Fighter.Unknown => 'Z',
                _ => throw new ArgumentOutOfRangeException(nameof(fighter))
            };
        }


        public static Fighter[] GetInferiors(this Fighter fighter)
        {
            return fighter switch
            {
                Fighter.Rock => new[] { Fighter.Rock, Fighter.Lizard, Fighter.Scissors },
                Fighter.Paper => new[] { Fighter.Rock, Fighter.Spock, Fighter.Paper },
                Fighter.Spock => new[] { Fighter.Rock, Fighter.Spock, Fighter.Scissors },
                Fighter.Scissors => new[] { Fighter.Paper, Fighter.Lizard, Fighter.Scissors },
                Fighter.Lizard => new[] { Fighter.Spock, Fighter.Lizard, Fighter.Paper },
            };
        }

        public static char GetOutcome(this string fight) => GetOutcome(fight[0], fight[1]);
        public static char GetOutcome(char first, char second)
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
                        'Y' => 'Y', // Spock vaporizes rock
                        'L' => 'R', // rock crushes lizard 
                        'X' => 'X', // available "wins" over all the others
                        'Z' => 'Z', // unknown "wins" against all others except X
                        _ => first
                    };

                case 'P':
                    return second switch
                    {
                        'R' => 'P', // paper beats rock
                        'S' => 'S', // scissors beats paper
                        'Y' => 'P', // paper disproves Spock
                        'L' => 'L', // lizard eats paper 
                        'X' => 'X', // available "wins" over all the others
                        'Z' => 'Z', // unknown "wins" against all others except X

                        _ => first
                    };

                case 'S':
                    return second switch
                    {
                        'P' => 'S', // scissors beats paper
                        'R' => 'R', // rock beats scissors
                        'Y' => 'Y', // Spock smashes scissors
                        'L' => 'S', // scissors decapitate lizards 
                        'X' => 'X', // available "wins" over all the others
                        'Z' => 'Z', // unknown "wins" against all others except X

                        _ => first
                    };

                case 'Y':
                    return second switch
                    {
                        'P' => 'P', // paper disproves Spock
                        'R' => 'Y', // Spock vaporizes rock
                        'S' => 'Y', // Spock smashes scissors
                        'L' => 'L', // lizard poisons Spock 
                        'X' => 'X', // available "wins" over all the others
                        'Z' => 'Z', // unknown "wins" against all others except X

                        _ => first
                    };

                case 'L':
                    return second switch
                    {
                        'P' => 'L', // lizard eats paper
                        'R' => 'R', // rock crushes lizard
                        'S' => 'S', // scissors decapitate lizards
                        'Y' => 'L', // lizard poisons Spock 
                        'X' => 'X', // available "wins" over all the others
                        'Z' => 'Z', // unknown "wins" against all others except X

                        _ => first
                    };

                case 'Z':
                    return second switch
                    {
                        // unknown "wins" against all others except X
                        'P' => 'Z', 
                        'R' => 'Z', 
                        'S' => 'Z', 
                        'Y' => 'Z', 
                        'L' => 'Z', 
                        'X' => 'X', 
                        _ => first
                    };

                case 'X':
                    // available "wins" over all the others
                    return 'X';
            
            }
            throw new ArgumentOutOfRangeException("Invalid fighting style");
        }




    }
}
