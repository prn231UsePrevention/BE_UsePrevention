namespace Dto.Response
{
    public class AssessmentAnswerDto
    {
        public int Id { get; set; }
        public int AssessmentResultId { get; set; }
        public int QuestionId { get; set; }
        public string Value { get; set; }
        
    }
}
