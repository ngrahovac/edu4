import React from 'react'
import { useState } from 'react';
import AddedPosition from '../comps/hats2/AddedPosition';
import Position from '../comps/hats2/Position';
import StudentHat from '../comps/hats2/StudentHat';
import StudentPosition from '../comps/hats2/StudentPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';

const Publish = () => {
    {/* TODO: remove static data */ }

    const [positions, setPositions] = useState([
        {
            title: ".NET Backend Developer",
            description: "You will be tasked with doing some development work on the back end",
            hat: {
                type: "Student",
                parameters: {
                    studyField: "Software Engineering"
                }
            }
        },
        {
            title: "React Frontend Developer",
            description: "You will be tasked with doing some development work on the front end and discussing design ideas",
            hat: {
                type: "Academic",
                parameters: {
                    researchField: "Neural Networks"
                }
            }
        }
    ]);

    function removePosition(positionToRemove) {
        let filteredPositions = positions.filter(p => p != positionToRemove);
        console.log(filteredPositions)
        setPositions(filteredPositions);
    }

    const left = (
        <>
            <div className='mb-12'>
                <SectionTitle title="Basic info"></SectionTitle>
            </div>
            <form>
                <div className='mb-8'>
                    <label>
                        <p>Title*</p>
                        <input
                            type="text"
                            className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                    </label>
                </div>

                <div className='mb-8'>
                    <label>
                        <p>Description*</p>
                        <textarea
                            rows={5}
                            maxLength={1000}
                            className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                    </label>
                </div>

                <div className='flex flex-row content-between justify-between'>
                    <label>
                        <p>Start date*</p>
                        <input type="date" class="mt-1 w-64 block rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        </input>
                    </label>
                    <label>
                        <p>End date*</p>
                        <input type="date" class="mt-1 w-64 block rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        </input>
                    </label>
                </div>
            </form>
        </>
    );

    const right = (
        <>
            <div className="mb-8">
                <SectionTitle title="Positions"></SectionTitle>
                <p>Describe the profiles of people you're looking to find and collaborate with</p>
            </div>

            <div className='mb-8'>
                <label>
                    <p>Position title*</p>
                    <input
                        type="text"
                        placeholder='e.g. .NET Backend Developer'
                        className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                </label>
            </div>

            <div className='mb-16'>
                <label>
                    <p>Position title*</p>
                    <select className="block w-full mt-1 rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        <option>Student</option>
                        <option>Academic / Researcher</option>
                    </select></label>
            </div>

            <div className='mb-2'>
                <SubsectionTitle title="Added positions"></SubsectionTitle>
            </div>
            {
                positions.length == 0 &&
                <p className='text-gray-500'>Currently there are no added positions.</p>
            }
            {
                positions.length > 0 &&
                <div className="mt-4"> {
                    positions.map(p => (
                        <div key={Math.random() * 1000}>
                            <div className='mb-2'>
                                {/*  <Position position={p}></Position> */}
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
        </>
    );

    return (
        <DoubleColumnLayout
            title="Publish a project"
            description="Describe the project or an idea you're working on, let the world know about it and find your people."
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default Publish