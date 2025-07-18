﻿namespace OJudge.Dtos
{
    public class CreateTopicDto
    {
        public required string Title { get; set; }
        public required string ShortTitle { get; set; }
        public string? Description { get; set; } = null;
    }
}
