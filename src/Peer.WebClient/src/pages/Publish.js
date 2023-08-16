import React, { useEffect } from 'react'
import { useState } from 'react';
import HatForm from '../comps/hat-forms/HatForm';
import AddedPosition from '../comps/hats2/AddedPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';
import NeutralButton from '../comps/buttons/NeutralButton';
import PrimaryButton from '../comps/buttons/PrimaryButton';
import { publish } from '../services/ProjectsService';
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react'
import BasicInfo from '../comps/publish/BasicInfo';
import Position from '../comps/publish/Position';


const Publish = () => {
    const [project, setProject] = useState({ positions: [] });
    const [position, setPosition] = useState({});

    const [validPosition, setValidPosition] = useState(false);
    const [validBasicInfo, setValidBasicInfo] = useState(false);
    const [validPositionCount, setValidPositionCount] = useState(false);

    const { getAccessTokenWithPopup } = useAuth0();

    useEffect(() => {
        setValidPositionCount(project.positions.length > 0);
    }, [project])


    function removePosition(positionToRemove) {
        let filteredPositions = project.positions.filter(p => p != positionToRemove);
        console.log(filteredPositions)
        setProject({ ...project, positions: filteredPositions });
    }

    function onPublishProject() {
        (async () => {
            if (validBasicInfo && validPositionCount) {
                try {
                    let token = await getAccessTokenWithPopup({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await publish(project, token);

                    if (result.outcome === successResult) {
                        // document.getElementById('user-action-success-toast').show();
                        // setTimeout(() => window.location.href = "/homepage", 1000);
                    } else if (result.outcome === failureResult) {
                        // document.getElementById('user-action-fail-toast').show();
                        // setTimeout(() => {
                        //     document.getElementById('user-action-fail-toast').close();
                        // }, 3000);
                    } else if (result.outcome === errorResult) {
                        // document.getElementById('user-action-fail-toast').show();
                        // setTimeout(() => {
                        //     document.getElementById('user-action-fail-toast').close();
                        // }, 3000);
                    }
                } catch (ex) {
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            }
        })();
    }

    const left = (
        <>
            <div className='mb-8'>
                <SectionTitle title="Basic info"></SectionTitle>
                <p className='h-8'></p>
            </div>
            <BasicInfo
                onValidChange={basicInfo => {
                    setProject({ ...project, ...basicInfo });
                    setValidBasicInfo(true);
                }}
                onInvalidChange={() => setValidBasicInfo(false)}>
            </BasicInfo>
        </>
    );

    const right = (
        <div className='relative pb-32'>
            <div className='relative pb-16'>
                <div className="mb-8">
                    <SectionTitle title="Positions"></SectionTitle>
                    <p className='h-8'>Describe the profiles of people you're looking to find and collaborate with</p>
                </div>

                <Position
                    onValidChange={position => {
                        setPosition(position);
                        setValidPosition(true);
                    }}
                    onInvalidChange={() => setValidPosition(false)}>
                </Position>

                <div className='absolute bottom-0 right-0'>
                    <NeutralButton
                        disabled={!validPosition}
                        text="Add"
                        onClick={() => {
                            if (validPosition)
                                setProject({ ...project, positions: [...project.positions, position] })
                        }}>
                    </NeutralButton>
                </div>
            </div>

            <div className='mb-2'>
                <SubsectionTitle title="Added positions"></SubsectionTitle>
                <p className='text-red-500 font-semibold h-8'>{`${validPositionCount ? "" : "Add at least one position when publishing a project."}`}</p>
            </div>
            {
                project.positions.length == 0 &&
                <p className='text-gray-500'>There are currently no added positions.</p>
            }
            {
                project.positions.length > 0 &&
                <div className="mt-4"> {
                    project.positions.map(p => (
                        <div key={Math.random() * 1000}>
                            <div className='mb-2'>
                                <AddedPosition
                                    position={p}
                                    onRemoved={() => removePosition(p)}>
                                </AddedPosition>
                            </div>
                        </div>)
                    )
                }
                </div>
            }

            <div className='absolute bottom-2 right-0'>
                <PrimaryButton
                    text="Publish"
                    onClick={onPublishProject}
                    disabled={!(validBasicInfo && validPositionCount)}>
                </PrimaryButton>
            </div>
        </div>
    );

    return (
        <DoubleColumnLayout
            title="Publish a project"
            description="Have an idea or a project you're working on? Let the world know and find your people!"
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default Publish