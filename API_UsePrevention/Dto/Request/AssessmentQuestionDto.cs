﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AssessmentQuestionDto
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }           
        public string? Options { get; set; }        
    }
}
