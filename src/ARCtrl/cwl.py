from __future__ import annotations
from collections.abc import Callable
from typing import Any
from .py.CWL.cwlprocessing_unit import CWLProcessingUnit
from .py.CWL.cwltypes import CWLType, FileInstance, DirectoryInstance, DirentInstance, InputEnumSchema, InputRecordField, InputRecordSchema, InputArraySchema, SchemaDefRequirementType, SoftwarePackage
from .py.CWL.parameter_reference import CWLParameterReference
from .py.CWL.inputs import CWLInput, InputBinding
from .py.CWL.outputs import CWLOutput, OutputBinding
from .py.CWL.tool_description import CWLToolDescription
from .py.CWL.workflow_description import CWLWorkflowDescription
from .py.CWL.workflow_steps import WorkflowStep, StepInput, StepOutput
from .py.CWL.requirements import Requirement, DockerRequirement, EnvironmentDef, ResourceRequirementInstance
