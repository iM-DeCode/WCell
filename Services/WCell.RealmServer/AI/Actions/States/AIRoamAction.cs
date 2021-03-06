using System;
using WCell.Constants.NPCs;
using WCell.Constants.Spells;
using WCell.Constants.Updates;
using WCell.RealmServer.AI.Actions.Movement;
using WCell.RealmServer.Entities;

namespace WCell.RealmServer.AI.Actions.States
{
	/// <summary>
	/// AI movemement action for roaming
	/// </summary>
	public class AIRoamAction : AIAction, IAIStateAction
	{
		public static int DefaultRoamSpellCastDelay = 30000;

		private DateTime lastSpellCast;

		private AIAction _strategy;

		public AIRoamAction(Unit owner)
			: base(owner)
		{
			MinimumRoamSpellCastDelay = DefaultRoamSpellCastDelay;
		}

		public AIRoamAction(Unit owner, AIAction roamAction) :
			base(owner)
		{
			Strategy = roamAction;
			MinimumRoamSpellCastDelay = DefaultRoamSpellCastDelay;
		}

		public int MinimumRoamSpellCastDelay
		{
			get;
			set;
		}

		/// <summary>
		/// The strategy to be used while roaming
		/// </summary>
		public AIAction Strategy
		{
			get { return _strategy; }
			set
			{
				_strategy = value;
			}
		}

		public override void Start()
		{
			// make sure we don't have Target nor Attacker
			m_owner.FirstAttacker = null;
			m_owner.Target = null;
			Strategy.Start();
		}

		public override void Update()
		{
			if (!m_owner.Brain.CheckCombat())
			{
				if (UsesSpells && HasSpellReady && m_owner.CanCastSpells && lastSpellCast + TimeSpan.FromMilliseconds(MinimumRoamSpellCastDelay) < DateTime.Now)
				{
					if (TryCastSpell())
					{
						lastSpellCast = DateTime.Now;
						m_owner.Movement.Stop();
						return;
					}
				}

				Strategy.Update();
			}
		}

		public override void Stop()
		{
			Strategy.Stop();
		}

		/// <summary>
		/// Tries to cast a Spell that is ready and allowed in the current context.
		/// </summary>
		/// <returns></returns>
		protected bool TryCastSpell()
		{
			var owner = (NPC)m_owner;

			foreach (var spell in owner.NPCSpells.ReadySpells)
			{
				if (!spell.HasHarmfulEffects && spell.CanCast(owner))
				{
					return m_owner.SpellCast.Start(spell) == SpellFailedReason.Ok;
				}
			}
			return false;
		}

		public override UpdatePriority Priority
		{
			get { return UpdatePriority.VeryLowPriority; }
		}
	}
}