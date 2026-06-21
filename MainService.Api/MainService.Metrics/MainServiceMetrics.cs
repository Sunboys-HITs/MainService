using Prometheus;

namespace MainService.Metrics;

public static class MainServiceMetrics
{
    public static readonly Counter HttpServerErrorsTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_http_server_errors_total",
            "Total number of HTTP 5xx responses and unhandled exceptions.",
            new CounterConfiguration
            {
                LabelNames = ["path", "status_code", "exception"],
            });

    public static readonly Counter SolutionsCreatedTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_solutions_created_total",
            "Total number of created solution submissions.",
            new CounterConfiguration
            {
                LabelNames = ["language"],
            });

    public static readonly Counter SolutionStatusUpdatedTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_solution_status_updated_total",
            "Total number of solution status updates.",
            new CounterConfiguration
            {
                LabelNames = ["status"],
            });

    public static readonly Counter CodeExecutionRequestsPublishedTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_code_execution_requests_published_total",
            "Total number of code execution requests published to RabbitMQ.",
            new CounterConfiguration
            {
                LabelNames = ["language"],
            });

    public static readonly Counter CodeExecutionRequestPublishFailuresTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_code_execution_request_publish_failures_total",
            "Total number of failed attempts to publish code execution requests to RabbitMQ.",
            new CounterConfiguration
            {
                LabelNames = ["language"],
            });

    public static readonly Histogram CodeExecutionRequestPublishDurationSeconds = Prometheus.Metrics
        .CreateHistogram(
            "main_service_code_execution_request_publish_duration_seconds",
            "Duration of publishing code execution requests to RabbitMQ.",
            new HistogramConfiguration
            {
                LabelNames = ["language"],
                Buckets = Histogram.ExponentialBuckets(0.005, 2, 12),
            });

    public static readonly Counter CodeExecutionResultsConsumedTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_code_execution_results_consumed_total",
            "Total number of code execution results consumed from RabbitMQ.",
            new CounterConfiguration
            {
                LabelNames = ["status"],
            });

    public static readonly Counter CodeExecutionResultFailuresTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_code_execution_result_failures_total",
            "Total number of code execution result handling failures.",
            new CounterConfiguration
            {
                LabelNames = ["reason"],
            });

    public static readonly Counter CodeExecutionTestsTotal = Prometheus.Metrics
        .CreateCounter(
            "main_service_code_execution_tests_total",
            "Total number of code execution tests reported by runner.",
            new CounterConfiguration
            {
                LabelNames = ["result"],
            });

    public static readonly Gauge CodeExecutionResultsInProgress = Prometheus.Metrics
        .CreateGauge(
            "main_service_code_execution_results_in_progress",
            "Number of code execution result messages currently being handled.");
}
