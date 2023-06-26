using System;
using System.Linq;

namespace PlaywrightTest.Core;

public static class FortuneCookie {
    private const string VALUES = @"The greatest risk is not taking one.
                         A short stranger will soon enter your life with blessings to share.
                         It's better to be alone sometimes.
                         Do not mistake temptation for opportunity.
                         You are very talented in many ways.
                         Serious trouble will bypass you.
                         Let the deeds speak.
                         Jealousy doesn't open doors, it closes them!
                         People are naturally attracted to you.
                         Fortune not found? Abort, Retry, Ignore.
                         A chance meeting opens new doors to success and friendship.
                         Your ability for accomplishment will follow with success.
                         You already know the answer to the questions lingering inside your head.
                         He who laughs at himself never runs out of things to laugh at.
                         Meeting adversity well is the source of your strength.
                         The man or woman you desire feels the same about you.
                         Land is always on the mind of a flying bird.
                         You cannot love life until you live the life you love.
                         You will live long enough to open many fortune cookies.
                         A fanatic is one who can't change his mind, and won't change the subject.
                         Never give up. You're not a failure if you don't give up.
                         A closed mouth gathers no feet.
                         A new voyage will fill your life with untold memories.
                         Change can hurt, but it leads a path to something better.
                         Meeting adversity well is the source of your strength.
                         Flattery will go far tonight.
                         A very attractive person has a message for you.
                         Love can last a lifetime, if you want it to.
                         Our deeds determine us, as much as we determine our deeds.
                         Now is the time to try something new.
                         LIFE CONSISTS NOT IN HOLDING GOOD CARDS, BUT IN PLAYING THOSE YOU HOLD WELL.
                         Actions speak louder than fortune cookies.
                         The world may be your oyster, but it doesn't mean you'll get its pearl.
                         The fortune you seek is in another cookie.
                         Enjoy the good luck a companion brings you.
                         If you feel you are right, stand firmly by your convictions.
                         The love of your life is stepping into your planet this summer.
                         Don't forget you are always on our minds.
                         You learn from your mistakes... You will learn a lot today.
                         Hard work pays off in the future, laziness pays off now.
                         Everyone agrees. You are the best.
                         You will travel to many exotic places in your lifetime.
                         You must try, or hate yourself for not trying.
                         You will conquer obstacles to achieve success.
                         Keep your eye out for someone special.
                         What ever you're goal is in life, embrace it visualize it, and for it will be yours.
                         You can make your own happiness.
                         Hidden in a valley beside an open stream- This will be the type of place where you will find your dream.
                         Joys are often the shadows, cast by sorrows.
                         A conclusion is simply the place where you got tired of thinking.
                         Be on the lookout for coming events; They cast their shadows beforehand.
                         The greatest love is self-love.
                         The man on the top of the mountain did not fall there.
                         If you look back, you'll soon be going that way.
                         The road to riches is paved with homework.
                         You will be hungry again in one hour.
                         You will marry your lover.
                         Your shoes will make you happy today.
                         When fear hurts you, conquer it and defeat it!
                         The greatest danger could be your stupidity.
                         Its amazing how much good you can do if you dont care who gets the credit.
                         Nothing astonishes men so much as common sense and plain dealing.
                         It is now, and in this world, that we must live.
                         A stranger, is a friend you have not spoken to yet.
                         You will be called in to fulfill a position of high honor and responsibility.
                         Wealth awaits you very soon.
                         If winter comes, can spring be far behind?
                         If you have something good in your life, don't let it go!
                         Your ability for accomplishment will follow with success.
                         A dream you have will come true.
                         Help! I am being held prisoner in a fortune cookie factory.
                         A smile is your passport into the hearts of others.
                         Because of your melodic nature, the moonlight never misses an appointment.
                         There is no greater pleasure than seeing your loved ones prosper.
                         Adversity is the parent of virtue.
                         Fortune favors the brave.
                         You will become great if you believe in yourself.                                   
";

    private static string[] items =
        VALUES.Split($"{Environment.NewLine}", StringSplitOptions.None)
            .Select(t => t.Trim())
            .ToArray();

    public static string GetMyFortune() {
        var r = new Random();
        return items[r.Next(0, items.Length)];
    }
}