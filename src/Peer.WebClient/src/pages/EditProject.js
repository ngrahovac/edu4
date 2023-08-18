import React, { useEffect } from 'react'
import { useState } from 'react';
import HatForm from '../comps/hat-forms/HatForm';
import AddedPosition from '../comps/publish/AddedPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';
import NeutralButton from '../comps/buttons/NeutralButton';
import PrimaryButton from '../comps/buttons/PrimaryButton';
import { addPositions, updateDetails } from '../services/ProjectsService';
import { getById, remove } from '../services/ProjectsService'

import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react'
import { Link, useParams } from 'react-router-dom';
import BasicInfoForm from '../comps/publish/BasicInfoForm';
import ProjectPositions from '../comps/project/ProjectPositions';
import PositionForm from '../comps/publish/PositionForm';
import DangerButton from '../comps/buttons/DangerButton';


const EditProject = () => {

    const { projectId } = useParams();

    const [originalProject, setOriginalProject] = useState(undefined);
    const [project, setProject] = useState(undefined);
    const [position, setPosition] = useState({});

    const [validPosition, setValidPosition] = useState(false);
    const [validBasicInfo, setValidBasicInfo] = useState(false);

    const [newPositions, setNewPositions] = useState([]);

    const [selectedPosition, setSelectedPosition] = useState(undefined);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    function fetchProject() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getById(projectId, token);

                if (result.outcome === successResult) {
                    var project = result.payload;

                    // sort positions by recommended first
                    const recommendedPositionSorter = (a, b) => {
                        if (a.recommended && !b.recommended) return -1;
                        if (!a.recommended && b.recommended) return +1;
                        return 0;
                    };

                    project.positions.sort(recommendedPositionSorter);

                    setProject(project);
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("neuspjesan status code");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("nesto je do mreze", result);
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log(ex);
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    useEffect(() => {
        fetchProject();
    }, [projectId]);

    useEffect(() => {
        setOriginalProject(project);
    }, [project])

    function removeNewPosition(positionToRemove) {
        let filteredPositions = newPositions.filter(p => p != positionToRemove);
        setNewPositions(filteredPositions);
    }

    function onAddPositions() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await addPositions(project.id, newPositions, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("network error");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log("error");
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    function onUpdateDetails() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await updateDetails(project.id, project.title, project.description, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("network error");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log("error");
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    function onPositionClosed() {
        // if successful, update the UI / state, don't refetch
    }

    function onPositionReopened() {
        // if successful, update the UI / state, don't refetch
    }

    function onExistingPositionRemoved() {
        // if successful, update the UI / state, don't refetch
    }

    const left = (
        project &&
        <>
            <div className='relative pb-16'>
                <div className='mb-8'>
                    <SectionTitle title="Basic info"></SectionTitle>
                    <p className='h-8'></p>
                </div>
                <BasicInfoForm
                    basicInfo={{ title: project.title, description: project.description }}
                    onValidChange={basicInfo => {
                        setProject({ ...project, ...basicInfo });
                        setValidBasicInfo(true);
                    }}
                    onInvalidChange={() => setValidBasicInfo(false)}>
                </BasicInfoForm>


                <div className='flex flex-row shrink-0 absolute bottom-2 right-0 space-x-2'>
                    <NeutralButton
                        text="Cancel"
                        onClick={() => { setProject({ ...project, title: originalProject.title, description: originalProject.description }) }}>
                    </NeutralButton>

                    <PrimaryButton
                        text="Update details"
                        onClick={onUpdateDetails}
                        disabled={!(validBasicInfo)}>
                    </PrimaryButton>
                </div>
            </div>
        </>
    );

    const right = (
        project &&
        <div className='relative pb-32'>
            <div className="mb-12">
                <SectionTitle title="Positions"></SectionTitle>
                <p>Describe the profiles of people you're looking to find and collaborate with</p>
            </div>

            {/* manage existing positions */}
            <div className='relative mb-12 pb-16'>
                <div className='mb-8'>
                    <SubsectionTitle title="Manage existing positions"></SubsectionTitle>
                </div>
                {
                    project.positions.length == 0 &&
                    <p className='text-gray-500'>Currently there are no added positions.</p>
                }
                {
                    project.positions.length > 0 &&
                    <div className="mt-4">
                        <ProjectPositions
                            positions={project.positions}
                            selectionEnabled={true}
                            onSelectedPositionChanged={(position) => { setSelectedPosition(position); }}>
                        </ProjectPositions>
                    </div>
                }

                {
                    project.positions.length > 0 &&
                    selectedPosition != undefined &&
                    <div className='absolute bottom-0 right-0 flex flex-row space-x-2'>
                        <DangerButton
                            text="Close"
                            disabled={position.open}
                            onClick={onPositionClosed}>
                        </DangerButton>

                        <NeutralButton
                            text="Reopen"
                            disabled={!position.open}
                            onClick={onPositionReopened}>
                        </NeutralButton>

                        <DangerButton
                            text="Remove"
                            onClick={onExistingPositionRemoved}>
                        </DangerButton>
                    </div>
                }
            </div>

            {/* add a new position */}
            <div className='relative mb-8 pb-16'>
                <div className='mb-4'>
                    <SubsectionTitle title="Add a new position"></SubsectionTitle>
                </div>
                <PositionForm
                    onValidChange={position => {
                        setPosition(position);
                        setValidPosition(true);
                    }}
                    onInvalidChange={() => setValidPosition(false)}>
                </PositionForm>

                <div className='absolute bottom-0 right-0'>
                    <NeutralButton
                        disabled={!validPosition}
                        text="Add"
                        onClick={() => {
                            if (validPosition)
                                setNewPositions([...newPositions, position])
                        }}>
                    </NeutralButton>
                </div>
            </div>

            {/* new positions */}
            <div>
                <div className='mb-4 mt-12'>
                    <SubsectionTitle title="New positions"></SubsectionTitle>
                </div>
                {
                    newPositions.length == 0 &&
                    <p className='text-gray-500'>Currently there are no added positions.</p>
                }
                {
                    newPositions.map(p => (
                        <div key={Math.random() * 1000}>
                            <div className='mb-2'>
                                {/*  <Position position={p}></Position> */}
                                <AddedPosition
                                    position={p}
                                    onRemoved={() => removeNewPosition(p)}>
                                </AddedPosition>
                            </div>
                        </div>)
                    )
                }

                <div className='flex flex-row shrink-0 absolute bottom-2 right-0 space-x-2'>
                    <NeutralButton
                        text="Cancel"
                        onClick={() => { setNewPositions([]) }}>
                    </NeutralButton>

                    <PrimaryButton
                        text="Update positions"
                        onClick={onAddPositions}
                        disabled={newPositions.length < 1}>
                    </PrimaryButton>
                </div>
            </div>
        </div>
    );

    return (
        project &&
        <DoubleColumnLayout
            title="Edit project"
            description=""
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default EditProject