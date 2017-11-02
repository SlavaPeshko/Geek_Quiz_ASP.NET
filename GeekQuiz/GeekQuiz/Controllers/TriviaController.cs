using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GeekQuiz.Models;

namespace GeekQuiz.Controllers
{
    public class TriviaController : ApiController
    {
	    private TriviaContext _context;

		public TriviaController(TriviaContext context)
		{
			_context = context;
		}

		// GET api/Trivia
		public async Task<IHttpActionResult> Get()
	    {
		    var userId = User.Identity.Name;

		    TriviaQuestion nextQuestion = await this.NextQuestionAsync(userId);

		    if (nextQuestion == null)
		    {
			    return this.NotFound();
		    }

		    return this.Ok(nextQuestion);
	    }

	    [ResponseType(typeof(TriviaAnswer))]
	    public async Task<IHttpActionResult> Post(TriviaAnswer answer)
	    {
		    if (!ModelState.IsValid)
		    {
			    return this.BadRequest(this.ModelState);
		    }

		    answer.UserId = User.Identity.Name;

		    var isCorrect = await this.StoreAsync(answer);
		    return this.Ok<bool>(isCorrect);
	    }

		protected override void Dispose(bool disposing)
	    {
		    if (disposing)
		    {
			    this._context.Dispose();
		    }
			base.Dispose(disposing);
	    }

		private async Task<TriviaQuestion> NextQuestionAsync(string userId)
		{
			var lastQuestionId = await this._context.TriviaAnswers
				.Where(a => a.UserId == userId)
				.GroupBy(a => a.QuestionId)
				.Select(g => new { QuestionId = g.Key, Count = g.Count() })
				.OrderByDescending(q => new { q.Count, QuestionId = q.QuestionId })
				.Select(q => q.QuestionId)
				.FirstOrDefaultAsync();

			var questionsCount = await this._context.TriviaQuestions.CountAsync();

			var nextQuestionId = (lastQuestionId % questionsCount) + 1;
			return await this._context.TriviaQuestions.FindAsync(CancellationToken.None, nextQuestionId);
		}

		private async Task<bool> StoreAsync(TriviaAnswer answer)
	    {
		    this._context.TriviaAnswers.Add(answer);

		    await this._context.SaveChangesAsync();
		    var selectedOption = await this._context.TriviaOptions
			    .FirstOrDefaultAsync(n => n.Id == answer.QuestionId);

		    return selectedOption.IsCorrect;
	    }
    }
}
