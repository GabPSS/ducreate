from aeneas.executetask import ExecuteTask
from aeneas.task import Task

def PerformAlign(speech_path: str, script_path: str, output_path: str):
    config_string = u"task_language=por|is_text_type=plain|os_task_file_format=json"
    task = Task(config_string=config_string)
    task.audio_file_path_absolute = speech_path
    task.text_file_path_absolute = script_path
    task.sync_map_file_path_absolute = output_path
    ExecuteTask(task).execute()
    task.output_sync_map_file()