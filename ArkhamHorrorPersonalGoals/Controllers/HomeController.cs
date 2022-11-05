using ArkhamHorrorPersonalGoals.Models;
using ArkhamHorrorPersonalGoals.Models.Error;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ArkhamHorrorPersonalGoals.Controllers
{
    [ResponseCache(NoStore = true)]
    public class HomeController : Controller
    {
        public const string WaitingForVisitors = "Waiting for visitors";

        // ReSharper disable once InconsistentNaming
        private static readonly ConcurrentDictionary<string, string> _visitors = new ConcurrentDictionary<string, string>();

        private static List<Goal> _goals;

        private static bool _goalsAreShown;

        private readonly ILogger<HomeController> _logger;

        private readonly Random _random = new Random();

        static HomeController()
        {
            ResetGoals();
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SetVisitorName(string name)
        {
            _logger.LogDebug($"User {name} has joined");

            var visitorId = GetIdFromCookie();

            var strippedName = string.Concat(name.Where(c => char.IsLetterOrDigit(c) || char.IsSeparator(c)));

            _visitors.AddOrUpdate(visitorId, strippedName, (_, _) => strippedName);

            if (_goals.All(x => x.Assignee != visitorId))
            {
                AssignRandomGoal(visitorId);
                AssignRandomGoal(visitorId);
            }

            return new JsonResult(true);
        }

        public IActionResult GetState()
        {
            if (_visitors.Count == 0)
            {
                return Content(WaitingForVisitors);
            }

            var visitorId = GetIdFromCookie();

            var visitorsAndGoals = new List<CurrentState>();

            foreach (var visitor in _visitors)
            {
                var assignedGoals = _goals.Where(x => x.Assignee == visitor.Key).ToList();
                var isYou = visitor.Key == visitorId;

                string stateText;
                if (assignedGoals.Count == 0)
                {
                    stateText = "Not yet been assigned goals";
                }
                else if (assignedGoals.Count > 1)
                {
                    stateText = "Not yet chosen a goal";
                }
                else if (!isYou && !_goalsAreShown)
                {
                    stateText = "Goal chosen, but hidden";
                }
                else
                {
                    stateText = assignedGoals[0].ToString();
                }

                visitorsAndGoals.Add(new CurrentState
                {
                    VisitorId = visitor.Key,
                    VisitorName = isYou ? "You" : visitor.Value,
                    DisplayGoal = stateText,
                    AssignedGoals = assignedGoals
                });
            }

            return PartialView("_VisitorGoals", visitorsAndGoals);
        }

        public IActionResult ChooseGoal(int id)
        {
            var visitorId = GetIdFromCookie();

            var assignedGoals = _goals.Where(x => x.Assignee == visitorId);

            //var chosenGoal = assignedGoals.First(x => x.Id == id);

            var otherGoal = assignedGoals.First(x => x.Id != id);
            otherGoal.Assignee = null;

            return new JsonResult(true);
        }

        public IActionResult ShowGoals()
        {
            _goalsAreShown = true;
            return new JsonResult(true);
        }

        public IActionResult Reset()
        {
            ResetGoals();
            _goalsAreShown = false;
            _visitors.Clear();
            return new JsonResult(true);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetIdFromCookie()
        {
            return Request.Cookies["ArkhamID"];
        }

        private static void ResetGoals()
        {
            //Credit to https://www.reddit.com/r/arkhamhorrorlcg/comments/w3uaj3/i_updatedadapted_some_gloomhaven_styled_secret/

            _goals = new List<Goal> {
                new Goal("Assassin", "Evade and defeat a monster on a single turn.", 1),
                new Goal("Bloodthirsty", "Defeat 3 or more enemies.", 1),
                new Goal("Careless", "During a single turn, take 2 attacks of opportunity.", 1),
                new Goal("Case Closed", "End the scenario with no clues on any location.", 1),
                new Goal("Cautious", "Reveal no locations during the scenario.", 1),
                new Goal("Committed", "Commit 3 or more cards during a single skill test.", 1),
                new Goal("Conservative", "End the scenario with 10 or fewer cards in your discard pile.", 1),
                new Goal("Crusader", "End the scenario with no enemies in play.", 1),
                new Goal("Cursed", "During the scenario, draw the auto-fail token at least once.", 1),
                new Goal("Detective", "Discover 6 or more clues.", 1),
                new Goal("Dominant", "End the scenario with 3 or more cards in the victory display.", 1),
                new Goal("Elusive", "Evade successfully 4 or more times.", 1),
                new Goal("Explorer", "Reveal 3 or more locations.", 1),
                new Goal("Focused", "In a single turn, discover 3 clues in the same location.", 1),
                new Goal("Fully Loaded", "End the scenario with two or more assets with current uses equal or higher than their starting uses.", 1),
                new Goal("Giant Slayer", "Land the killing blow to defeat an Elite enemy.", 1),
                new Goal("Helping Hand", "Heal a health or horror on an investigator other than yourself.", 1),
                new Goal("Introvert", "End the scenario without playing any ally cards.", 1),
                new Goal("\"Keep absolutely still\"", "Defeat an enemy while it is engaged with another investigator.", 1),
                new Goal("Masochist", "End the scenario with 2 or fewer health remaining.", 1),
                new Goal("On the Edge", "End the scenario with 2 or fewer sanity remaining.", 1),
                new Goal("Pacifist", "Don't defeat any enemies.", 1),
                new Goal("Pauper", "End the scenario with 2 or fewer resources.", 1),
                new Goal("Ready for anything", "End the scenario with at least 1 accessory, body, hand, arcane, and ally slot equipped.", 1),
                new Goal("Saviour", "Over the course of the scenario, commit at least 4 cards to other investigator's skill tests.", 1),
                new Goal("Seasoned", "End the scenario with 20 or more cards in your discard pile.", 1),
                new Goal("Selfish", "Don't commit any cards to other investigator's skill tests.", 1),
                new Goal("Sluggish", "Don't play any cards with the fast keyword during the scenario.", 1),
                new Goal("Strong of Body", "Never fall below half health (rounded down).", 1),
                new Goal("Strong of Will", "Never fall below half sanity (rounded down).", 1),
                new Goal("Superstar", "Success on a skill test by 6 or more.", 1),
                new Goal("\"Pick on someone your own size!\"", "Use the engage action on enemies engaged with other investigators at least 3 times.", 1),
                new Goal("Unscathed", "Take no damage from enemies during the scenario.", 1),
                new Goal("Wealthy", "End the scenario with 10 or more resources.", 1),
                new Goal("Surrounded", "Be engaged with 3 or more enemies at the same time.", 2),
                new Goal("\"I don't need luck!\"", "Commit no cards other than skill cards during this scenario.", 1),
                new Goal("\"I'm going to need a bigger gun\"", "Kill 5 enemies during this scenario.", 2),
                new Goal("\"See ya!\"", "Be the first player to resign without being defeated during this scenario.", 1),
                new Goal("\"Better you than me\"", "You discard an ally from play by reaching their damage threshold.", 1),
                new Goal("Emotional Support", "You discard an ally from play by reaching their sanity threshold.", 1),
                new Goal("Two for the price of one", "Kill two enemies in the same round", 1),
                new Goal("Wiolence isn't the answer", "Don't deal any damage to enemies", 1),
                new Goal("Over-estimate", "Commit 6 skill icons to a single test", 1),
                new Goal("Consistent", "Succeed on three skill tests in one turn", 1),
                new Goal("Out of supplies", "Don't take any 'draw' or 'gain resource' actions during the scenario", 2),
                new Goal("Overkill", "Defeat an enemy by dealing at least one damage in excess of their health", 1),
                new Goal("Exposed", "Don't assign any damage or horror to your assets during the scenario", 2),
                new Goal("Foolhardy", "Don't assign any damage to your assets during the scenario", 1),
                new Goal("Eyes Wide Open", "Don't assign any horror to your assets during the scenario", 1),
                new Goal("Oops!", "Fail a skill test while attacking an enemy engaged with another investigator", 2),
                new Goal("Cursed Luck", "Fail a skill test after comitting 2 or more cards to it", 2),
                new Goal("I can handle it", "Never play or commit a card during the Mythos phase", 2),
                new Goal("Bereft", "Don't commit any cards on skill tests on treacheries", 2),
                new Goal("Dauntless", "Don't take any horror during the mythos phase", 1),
                new Goal("Enter the Crucible", "Don't take any damage during the Mythos phase", 1),
                new Goal("Resolute", "Don't take any damage or horror during the Mythos phase", 2),
            };

            for (var i = 0; i < _goals.Count; i++)
            {
                _goals[i].Id = i + 1;
            }
        }

        private void AssignRandomGoal(string assignee)
        {
            lock (_goals)
            {
                var nonAssigned = _goals.Where(x => x.Assignee == null);
                var goal = nonAssigned.ElementAt(_random.Next(nonAssigned.Count()));
                goal.Assignee = assignee;
            }
        }
    }
}