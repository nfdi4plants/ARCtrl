from __future__ import annotations
from collections.abc import Callable
from typing import Any
# Unions are transpiled into an implementation class holding the members and a
# type alias over the case classes. The public name has to be the implementation
# class, otherwise members and `isinstance` are unavailable. See ARCtrl/__init__.py.
from .py.CWL.cwlprocessing_unit import _CWLProcessingUnit as CWLProcessingUnit
from .py.CWL.cwltypes import _CWLType as CWLType, FileInstance, DirectoryInstance, DirentInstance, InputEnumSchema, InputRecordField, InputRecordSchema, InputArraySchema, SchemaDefRequirementType, SoftwarePackage
from .py.CWL.parameter_value import _CWLParameterValue as CWLParameterValue, CWLParameterRecordField
from .py.CWL.parameter_reference import CWLParameterReference
from .py.CWL.inputs import CWLInput, InputBinding
from .py.CWL.outputs import CWLOutput, OutputBinding
from .py.CWL.tool_description import CWLToolDescription
from .py.CWL.workflow_description import CWLWorkflowDescription
from .py.CWL.workflow_steps import WorkflowStep, StepInput, _StepOutput as StepOutput
from .py.CWL.requirements import _Requirement as Requirement, DockerRequirement, EnvironmentDef, ResourceRequirementInstance
