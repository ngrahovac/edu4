import PositionCard from './PositionCard';
import ProjectTitle from './ProjectTitle';
import Nina from '../temp/Nina';
import Collaborators from '../temp/Collaborators';
import TertiaryButton from '../buttons/TertiaryButton'

const ProjectCard = (props) => {

    const {
        project,
        ownHats = undefined
    } = props;

    const maxPositionsShown = 2;

    let timeDifference = Date.now() - new Date(project.datePosted);
    let daysSince = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    let notShownPositionsCount = 0;

    if (project.recommended) {
        let recommendedPositionsCount = project.positions.filter(p => p.recommended).length;
        if (recommendedPositionsCount > maxPositionsShown) {
            notShownPositionsCount = recommendedPositionsCount - maxPositionsShown;
        }
    } else {
        let positionsCount = project.positions.length;
        if (positionsCount > maxPositionsShown) {
            notShownPositionsCount = positionsCount - maxPositionsShown;
        }
    }

    return (
        <div className='bg-white rounded-3xl cursor-pointer border border-gray-200'>
            <div className="flex flex-col gap-y-8 px-16 py-12">
                <div className='flex flex-row justify-between'>
                    <div className='flex gap-x-4'>
                        <Nina></Nina>
                        <div className='flex flex-col'>
                            <p className='text-gray-500 font-semibold'>{project.author.fullName}</p>
                            <p className='text-gray-500 uppercase text-sm'>{daysSince} days ago</p>
                        </div>
                    </div>

                    <Collaborators></Collaborators>
                </div>

                <div className='flex flex-col gap-y-2'>
                    <ProjectTitle>{project.title}</ProjectTitle>
                    <p className='text-justify text-gray-800'>{project.description}</p>
                </div>

                <div className='flex flex-col gap-y-4'>

                    <div className='flex flex-col space-y-2'>
                        {
                            project.recommended &&

                            project.positions.sort(p => !p.recommended).slice(0, maxPositionsShown).map((p, index) => <div key={index}>
                                <PositionCard position={p} ownHats={ownHats}></PositionCard>
                            </div>)
                        }

                        {
                            !project.recommended &&

                            project.positions.slice(0, maxPositionsShown).map((p, index) => <div key={index}>
                                <PositionCard position={p} ownHats={ownHats}></PositionCard>
                            </div>)
                        }
                    </div>
                </div>

                <div className='flex flex-row-reverse'>
                    <TertiaryButton text="Learn more"></TertiaryButton>
                </div>
            </div>
        </div>
    )
}

export default ProjectCard